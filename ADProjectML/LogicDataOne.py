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
df_extracted = df.iloc[:,[3,4,9,10,12,13,14]]
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
        itemName_array = df_extracted['StationeryName'].unique()
        itemSelectDateRQ_list = []
        itemOneDayRQ_list = []
        itemDate_list = []
        itemNextDate_list = []
        itemSelectedDate_list =[]
        for item in itemName_array:
            df_filterByValue = df_extracted[df_extracted['StationeryName'].str.match(item)] 
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
            one_day_RQprediction = linReg.predict([[df_filterByValue.iloc[-1,9]+1]])
            one_day_RQ_difference = one_day_RQprediction - df_filterByValue.iloc[-1,8]
            requisition_msg_1 = '0'
            
            if(quantity_difference > 0):
                itemSelectDateRQ_list.append(str(int(quantity_difference)))
            else:
                itemSelectDateRQ_list.append(requisition_msg_1)
            if(one_day_RQ_difference > 0):
                itemOneDayRQ_list.append(str(int(one_day_RQ_difference)))
            else:
                itemOneDayRQ_list.append(requisition_msg_1)
            itemDate_list.append(str(date.fromordinal(df_filterByValue.iloc[-1,9])))
            itemNextDate_list.append(str(date.fromordinal(df_filterByValue.iloc[-1,9]+1)))
            itemSelectedDate_list.append(str(date.fromordinal(input_date_ordinal)))
            
            
        itemRQ_array = np.asarray(itemSelectDateRQ_list)
        itemOneDayRQ_array = np.asarray(itemOneDayRQ_list)
        itemDate_array = np.asarray(itemDate_list)
        itemNextDate_array = np.asarray(itemNextDate_list)
        itemSelectedDate_array = np.asarray(itemSelectedDate_list)
        df_AllRQ = pd.DataFrame({'Stationery Name': itemName_array, 'Most Recent Requisition Date': itemDate_array})
        df_AllRQ['Next Day of Most Recent'] = itemNextDate_array
        df_AllRQ['Estimated One Day Requisition Quantity'] = itemOneDayRQ_array
        df_AllRQ['Selected Requisition day'] = itemSelectedDate_array
        df_AllRQ['Estimated Requisition Quantity'] = itemRQ_array
        df_AllRQ_json = df_AllRQ.to_json(orient = 'values')
        return df_AllRQ_json

# In[12]:
linReg = LinearRegression()
LRT = LRTraining()
f = open('LogicDataOne.pkl', 'wb')

pickle.dump(LRT, f)
pickle.dump(linReg, f)

f.close()