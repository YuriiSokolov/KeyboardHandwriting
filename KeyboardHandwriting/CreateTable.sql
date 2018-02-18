CREATE Table Data(ID decimal(9,0) IDENTITY(1, 1) CONSTRAINT OwnersPrimary PRIMARY KEY,
Login varchar(50) NOT NULL, MinEnterSpeed varchar(50) NOT NULL, MaxEnterSpeed varchar(50) NOT NULL, 
MinClicksSpeed varchar(50) NOT NULL, MaxClicksSpeed varchar(50) NOT NULL,
counterError varchar(50) NOT NULL);
select * from Data
delete from data where id = '2'