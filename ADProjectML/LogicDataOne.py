#!/usr/bin/env python
# coding: utf-8

# # Machine Learning Documentation

# ## Load Data

# In[1]:


#import relevant packages
import pandas as pd
import numpy as np
import matplotlib.pyplot as plt
import seaborn as sns
import pickle
from datetime import datetime
from datetime import date
from sklearn import linear_model
from sklearn.model_selection import train_test_split
from sklearn.metrics import mean_squared_error, r2_score
from sklearn.metrics import accuracy_score
from sklearn.linear_model import LinearRegression


# In[2]:


#loading data
df = pd.read_csv('MockDataV2.csv')
df.head()


# In[3]:


#checking datatypes
df.dtypes


# ### Data Pre-processing

# In[4]:


#adding a new column(TotalPrice_new) for converting TotalPrice into datatype float64.
df['TotalPrice_new'] = df['TotalPrice'].apply(lambda x: x.replace('$','')).apply(lambda x: x.replace(',','')).astype(np.float64)
df


# In[5]:


#extract the necessary columns
df_extracted = df.iloc[:,[1,2,3,4,9,10,12,13,14]]
df_extracted.head()


# In[6]:


df_extracted['Dates'] = df_extracted['Dates'].astype('datetime64[ns]')
df_extracted


# In[7]:


df_extracted.info()


# In[8]:


#list down all the stationery types
TypeArray = df_extracted.StationeryType.unique()
TypeArray

#replace stationery type name with an integer 
def Trans_StationeryType(x):
    for y in range(0,len(TypeArray)):
        if x == TypeArray[y]:
            return y+1

#adding a new column for the stationery type in terms of integer
df_extracted['TransStationeryType'] = df_extracted['StationeryType'].apply(Trans_StationeryType)
df_extracted.head()


# In[9]:


#groupby Stationery Name & Type & Supplier and sum the quantity and total price
df_new = df_extracted.groupby(['StationeryName','StationeryType','SupplierName']).agg({'RequisitionQuantity':'sum','TotalPrice_new':'sum'}) 
df_new


# In[10]:


df_extracted.info()


# ## LR Model Training

# In[16]:


class LRTraining:

    
    @classmethod
    def SelectByName(cls, InputName):
        df_filterByValue = df_extracted[df_extracted['StationeryName'].str.match(InputName)] #display item with selected name
        df_filterByValue['Cumulative_RQ'] = df_filterByValue['RequisitionQuantity'].cumsum(axis=0) #add a cumulative requisition quantity column
        df_filterByValue['Dates_ordinal'] = df_filterByValue['Dates'].apply(lambda date: date.toordinal()) #convert Dates from object to datetime
        df_filterByValue_json = df_filterByValue.to_json(orient = 'table')
        return df_filterByValue_json
    
    @classmethod
    def LinearRegressionGraph(cls, InputName):
        df_filterByValue = df_extracted[df_extracted['StationeryName'].str.match(InputName)] 
        df_filterByValue['Cumulative_RQ'] = df_filterByValue['RequisitionQuantity'].cumsum(axis=0) 
        df_filterByValue['Dates_ordinal'] = df_filterByValue['Dates'].apply(lambda date: date.toordinal()) 
        ax = sns.regplot(x='Dates_ordinal', y = 'Cumulative_RQ', data = df_filterByValue)
        ax.set_xlim(df_filterByValue['Dates_ordinal'].min() - 1, df_filterByValue['Dates_ordinal'].max() + 1)
        ax.set_ylim(0, df_filterByValue['Cumulative_RQ'].max() + 1)
        ax.set_xlabel('Time')
        ax.set_ylabel('Requisition Quantity')
        new_labels = [date.fromordinal(int(item)) for item in ax.get_xticks()]
        ax.set_xticklabels(new_labels)
        plt.xticks(rotation=45)
        return ax
    
    @classmethod
    def LinearRegressionScore(cls, InputName):
        df_filterByValue = df_extracted[df_extracted['StationeryName'].str.match(InputName)] 
        df_filterByValue['Cumulative_RQ'] = df_filterByValue['RequisitionQuantity'].cumsum(axis=0) 
        df_filterByValue['Dates_ordinal'] = df_filterByValue['Dates'].apply(lambda date: date.toordinal()) 
        X = df_filterByValue[['Dates_ordinal']]
        y = df_filterByValue['Cumulative_RQ']
        X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
        linReg = LinearRegression()
        linReg.fit(X_train, y_train)
        y_prep = linReg.predict(X_test)
        the_score = r2_score(y_test, y_prep)
        return the_score
    
    @classmethod
    def EstimateRequisitionQuantity(cls, InputYear,InputMonth,InputDay,InputName):
        df_filterByValue = df_extracted[df_extracted['StationeryName'].str.match(InputName)] 
        df_filterByValue['Cumulative_RQ'] = df_filterByValue['RequisitionQuantity'].cumsum(axis=0) 
        df_filterByValue['Dates_ordinal'] = df_filterByValue['Dates'].apply(lambda date: date.toordinal()) 
        input_date = date(InputYear,InputMonth,InputDay)
        input_date_ordinal = input_date.toordinal()
        X = df_filterByValue[['Dates_ordinal']]
        y = df_filterByValue['Cumulative_RQ']
        X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
        linReg = LinearRegression()
        linReg.fit(X_train, y_train)
        requisition_quantity_prediction = linReg.predict([[input_date_ordinal]])
        quantity_difference = requisition_quantity_prediction - df_filterByValue.iloc[-1,8]
        requisition_msg_1 = 'May not receive ANY requisition by this date!'
        requisition_msg_2 = 'Estimated requisition quantity: '
        if(quantity_difference > 0):
            return requisition_msg_2 + str(int(quantity_difference))
        else:
            return requisition_msg_1

    @classmethod    
    def NextRequisitionDay(cls, InputName):
        df_filterByValue = df_extracted[df_extracted['StationeryName'].str.match(InputName)] 
        df_filterByValue['Cumulative_RQ'] = df_filterByValue['RequisitionQuantity'].cumsum(axis=0) 
        df_filterByValue['Dates_ordinal'] = df_filterByValue['Dates'].apply(lambda date: date.toordinal()) 
        latest_requisition_quantity = df_filterByValue.iloc[-1,8]
        latest_requisition_date = df_filterByValue.iloc[-1,9]
        estimate_next_requisition = latest_requisition_date
        X = df_filterByValue[['Dates_ordinal']]
        y = df_filterByValue['Cumulative_RQ']
        X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
        linReg = LinearRegression()
        linReg.fit(X_train, y_train)
        current_requisition_quantity = linReg.predict([[estimate_next_requisition]])
        if(current_requisition_quantity <= latest_requisition_quantity):
            while(current_requisition_quantity <= latest_requisition_quantity):
                estimate_next_requisition = estimate_next_requisition +1
                current_requisition_quantity = linReg.predict([[estimate_next_requisition]])
            return date.fromordinal(estimate_next_requisition)
        else: 
            return 'You should receive requisition by Now!'

    @classmethod
    def EstimateAllRequisitionQuantity(cls, InputYear, InputMonth, InputDay):
        itemId_array = df_extracted['TempIndex'].unique()
        itemCode_array = df_extracted['ItemNumber'].unique()
        itemSelectDateRQ_list = []
        itemOneMonthRQ_list = []
        itemDate_list = []
        itemMonthEndDate_list = []
        itemSelectedDate_list =[]
        for item in itemCode_array:
            df_filterByValue = df_extracted[df_extracted['ItemNumber'].str.match(item)] 
            df_filterByValue['Cumulative_RQ'] = df_filterByValue['RequisitionQuantity'].cumsum(axis=0) 
            df_filterByValue['Dates_ordinal'] = df_filterByValue['Dates'].apply(lambda date: date.toordinal()) 
            input_date = date(InputYear,InputMonth,InputDay)
            if(InputMonth == 4) or (InputMonth == 6) or (InputMonth == 9) or (InputMonth == 11):
                input_date_ordinal = input_date.toordinal()
                final_firstdayofmonth = date(InputYear,InputMonth,1)
                final_firstdayofmonth_ordinal = final_firstdayofmonth.toordinal()
                final_lastdayofmonth = date(InputYear,InputMonth,30)
                final_lastdayofmonth_ordinal = final_lastdayofmonth.toordinal()
                X = df_filterByValue[['Dates_ordinal']]
                y = df_filterByValue['Cumulative_RQ']
                X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
                linReg = LinearRegression()
                linReg.fit(X_train, y_train)
                requisition_quantity_prediction = linReg.predict([[input_date_ordinal]])
                requisition_quantity_prediction_on_firstdayofmonth = linReg.predict([[final_firstdayofmonth_ordinal]])
                requisition_quantity_prediction_on_lastdayofmonth = linReg.predict([[final_lastdayofmonth_ordinal]])
#                 quantity_difference = requisition_quantity_prediction - df_filterByValue.iloc[-1,8]
#                 one_Month_RQprediction = linReg.predict([[df_filterByValue.iloc[-1,9]+29]])
#                 one_Month_RQ_difference = one_Month_RQprediction - df_filterByValue.iloc[-1,8]
                one_Month_RQ_difference = requisition_quantity_prediction_on_lastdayofmonth - requisition_quantity_prediction_on_firstdayofmonth
                requisition_msg_1 = '0'

#                 if(quantity_difference > 0):
#                     itemSelectDateRQ_list.append(str(int(quantity_difference)))
#                 else:
#                     itemSelectDateRQ_list.append(requisition_msg_1)
                if(one_Month_RQ_difference > 0):
                    itemOneMonthRQ_list.append(str(int(one_Month_RQ_difference)))
                else:
                    itemOneMonthRQ_list.append(requisition_msg_1)
                itemDate_list.append(str(final_firstdayofmonth))
                itemMonthEndDate_list.append(str(final_lastdayofmonth))
                itemSelectedDate_list.append(str(date.fromordinal(input_date_ordinal)))
            elif(InputMonth == 1) or (InputMonth == 3) or (InputMonth == 5) or (InputMonth == 7) or (InputMonth == 8) or (InputMonth == 10) or (InputMonth == 12):
                input_date_ordinal = input_date.toordinal()
                final_firstdayofmonth = date(InputYear,InputMonth,1)
                final_firstdayofmonth_ordinal = final_firstdayofmonth.toordinal()
                final_lastdayofmonth = date(InputYear,InputMonth,31)
                final_lastdayofmonth_ordinal = final_lastdayofmonth.toordinal()
                X = df_filterByValue[['Dates_ordinal']]
                y = df_filterByValue['Cumulative_RQ']
                X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
                linReg = LinearRegression()
                linReg.fit(X_train, y_train)
                requisition_quantity_prediction = linReg.predict([[input_date_ordinal]])
                requisition_quantity_prediction_on_firstdayofmonth = linReg.predict([[final_firstdayofmonth_ordinal]])
                requisition_quantity_prediction_on_lastdayofmonth = linReg.predict([[final_lastdayofmonth_ordinal]])
#                 quantity_difference = requisition_quantity_prediction - df_filterByValue.iloc[-1,8]
#                 one_Month_RQprediction = linReg.predict([[df_filterByValue.iloc[-1,9]+30]])
#                 one_Month_RQ_difference = one_Month_RQprediction - df_filterByValue.iloc[-1,8]
                one_Month_RQ_difference = requisition_quantity_prediction_on_lastdayofmonth - requisition_quantity_prediction_on_firstdayofmonth
                requisition_msg_1 = '0'

#                 if(quantity_difference > 0):
#                     itemSelectDateRQ_list.append(str(int(quantity_difference)))
#                 else:
#                     itemSelectDateRQ_list.append(requisition_msg_1)
                if(one_Month_RQ_difference > 0):
                    itemOneMonthRQ_list.append(str(int(one_Month_RQ_difference)))
                else:
                    itemOneMonthRQ_list.append(requisition_msg_1)
                itemDate_list.append(str(final_firstdayofmonth))
                itemMonthEndDate_list.append(str(final_lastdayofmonth))
                itemSelectedDate_list.append(str(date.fromordinal(input_date_ordinal)))
            else:
                if(InputYear % 4 == 0):
                    input_date_ordinal = input_date.toordinal()
                    final_firstdayofmonth = date(InputYear,InputMonth,1)
                    final_firstdayofmonth_ordinal = final_firstdayofmonth.toordinal()
                    final_lastdayofmonth = date(InputYear,InputMonth,29)
                    final_lastdayofmonth_ordinal = final_lastdayofmonth.toordinal()
                    X = df_filterByValue[['Dates_ordinal']]
                    y = df_filterByValue['Cumulative_RQ']
                    X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
                    linReg = LinearRegression()
                    linReg.fit(X_train, y_train)
                    requisition_quantity_prediction = linReg.predict([[input_date_ordinal]])
                    requisition_quantity_prediction_on_firstdayofmonth = linReg.predict([[final_firstdayofmonth_ordinal]])
                    requisition_quantity_prediction_on_lastdayofmonth = linReg.predict([[final_lastdayofmonth_ordinal]])
#                     quantity_difference = requisition_quantity_prediction - df_filterByValue.iloc[-1,8]
#                     one_Month_RQprediction = linReg.predict([[df_filterByValue.iloc[-1,9]+28]])
#                     one_Month_RQ_difference = one_Month_RQprediction - df_filterByValue.iloc[-1,8]
                    one_Month_RQ_difference = requisition_quantity_prediction_on_lastdayofmonth - requisition_quantity_prediction_on_firstdayofmonth
                    requisition_msg_1 = '0'

#                     if(quantity_difference > 0):
#                         itemSelectDateRQ_list.append(str(int(quantity_difference)))
#                     else:
#                         itemSelectDateRQ_list.append(requisition_msg_1)
                    if(one_Month_RQ_difference > 0):
                        itemOneMonthRQ_list.append(str(int(one_Month_RQ_difference)))
                    else:
                        itemOneMonthRQ_list.append(requisition_msg_1)
                    itemDate_list.append(str(final_firstdayofmonth))
                    itemMonthEndDate_list.append(str(final_lastdayofmonth))
                    itemSelectedDate_list.append(str(date.fromordinal(input_date_ordinal)))
                else:
                    input_date_ordinal = input_date.toordinal()
                    final_firstdayofmonth = date(InputYear,InputMonth,1)
                    final_firstdayofmonth_ordinal = final_firstdayofmonth.toordinal()
                    final_lastdayofmonth = date(InputYear,InputMonth,28)
                    final_lastdayofmonth_ordinal = final_lastdayofmonth.toordinal()
                    X = df_filterByValue[['Dates_ordinal']]
                    y = df_filterByValue['Cumulative_RQ']
                    X_train, X_test, y_train, y_test = train_test_split(X,y, random_state = 0)
                    linReg = LinearRegression()
                    linReg.fit(X_train, y_train)
                    requisition_quantity_prediction = linReg.predict([[input_date_ordinal]])
                    requisition_quantity_prediction_on_firstdayofmonth = linReg.predict([[final_firstdayofmonth_ordinal]])
                    requisition_quantity_prediction_on_lastdayofmonth = linReg.predict([[final_lastdayofmonth_ordinal]])
#                     quantity_difference = requisition_quantity_prediction - df_filterByValue.iloc[-1,8]
#                     one_Month_RQprediction = linReg.predict([[df_filterByValue.iloc[-1,9]+27]])
#                     one_Month_RQ_difference = one_Month_RQprediction - df_filterByValue.iloc[-1,8]
                    one_Month_RQ_difference = requisition_quantity_prediction_on_lastdayofmonth - requisition_quantity_prediction_on_firstdayofmonth
                    requisition_msg_1 = '0'

#                     if(quantity_difference > 0):
#                         itemSelectDateRQ_list.append(str(int(quantity_difference)))
#                     else:
#                         itemSelectDateRQ_list.append(requisition_msg_1)
                    if(one_Month_RQ_difference > 0):
                        itemOneMonthRQ_list.append(str(int(one_Month_RQ_difference)))
                    else:
                        itemOneMonthRQ_list.append(requisition_msg_1)
                    itemDate_list.append(str(final_firstdayofmonth))
                    itemMonthEndDate_list.append(str(final_lastdayofmonth))
                    itemSelectedDate_list.append(str(date.fromordinal(input_date_ordinal)))
            
        itemRQ_array = np.asarray(itemSelectDateRQ_list)
        itemOneMonthRQ_array = np.asarray(itemOneMonthRQ_list)
        itemDate_array = np.asarray(itemDate_list)
        itemMonthEndDate_array = np.asarray(itemMonthEndDate_list)
        itemSelectedDate_array = np.asarray(itemSelectedDate_list)
        df_AllRQ = pd.DataFrame({'Id':itemId_array,'Reorder level and quantity': itemOneMonthRQ_array})
 #       df_AllRQ['Reorder level and quantity'] = itemOneMonthRQ_array
#        df_AllRQ['Last day of the Month'] = itemMonthEndDate_array
#        df_AllRQ['1st Day of the Moneth'] = itemDate_array
#        df_AllRQ['Estimated One Day Requisition Quantity'] = itemOneMonthRQ_array
#         df_AllRQ['Selected Requisition day'] = itemSelectedDate_array
#         df_AllRQ['Estimated Requisition Quantity'] = itemRQ_array
        df_AllRQ_json = df_AllRQ.to_json(orient = 'values')
        return df_AllRQ_json

# In[12]:
linReg = LinearRegression()
LRT = LRTraining()
f = open('LogicDataOne.pkl', 'wb')

pickle.dump(LRT, f)
pickle.dump(linReg, f)

f.close()