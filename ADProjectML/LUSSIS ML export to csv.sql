select s.Id as TempIndex, s.Code as ItemNumber, 
c.Type as StationeryType, s.Description as StationeryName, s.ReorderLevel as ReorderLevel, 
s.ReorderQuantity as ReorderQty, s.UnitOfMeasure as UOM, st.Price as Price, su.Name as SupplierName, 
r.DateTime as Dates, 
rd.QuantityDelivered as RequisitionQuantity, (st.Price*rd.QuantityDelivered) as TotalPrice
from Requisition r, RequisitionDetail rd, Stationery s, SupplierTender st, Category c, Supplier su
where rd.RequisitionId = r.Id and rd.StationeryId = s.Id and s.CategoryId = c.Id and st.StationeryId = s.Id and st.Rank = 1 and su.Id = st.SupplierId