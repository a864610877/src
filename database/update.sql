--alter table users add babyName nvarchar(100)
--alter table users add babySex int
--alter table users add babyBirthDate datetime
--alter table users add openId nvarchar(200)

--drop table Tickets
--使用范围 为空不限制使用，否则绑定商户号
--alter table accounts add useScope nvarchar(200) default('')
--create table Tickets
--(
--   id int identity(1,1) primary key,
--   userId int,
--   orderNo varchar(100),--订单号
--   AdmissionTicketId int,
--   Code nvarchar(200),
--   ExpiredDate datetime,
--   Price decimal(18,2),
--   adultNum int,
--   childNum int,
--   useScope nvarchar(200) default(''),--使用范围 为空不限制使用，否则绑定商户号,指定门店消费
--   [State] int,--1 未使用 2 已使用 3 已过期
--   BuyTime datetime
--)

--go

--GO
--create PROCEDURE [dbo].[P_getTickets]
-- @userId int=null,
-- @pageIndex INT,
-- @pageSize INT
--AS
--BEGIN
--SELECT count(1) as Total from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
--(@userId is null or t.UserId = @userId)

-- select * from  (select Row_Number() OVER(order by t.code )AS RowNum , t.*,u.name as 'TicketName' from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
--(@userId is null or t.UserId = @userId)
--)t
--where t.RowNum > (@pageIndex -1) * @pageSize 
--         AND t.RowNum <=   @pageIndex * @pageSize
--END


--GO

--CREATE TABLE [dbo].[SetWeChats](
--	[setWeChatId] [int] IDENTITY(1,1) NOT NULL,
--	[appID] [nvarchar](200) NULL,
--	[appSecret] [nvarchar](200) NULL,
--	[token] [nvarchar](200) NULL,
--	[access_token] [nvarchar](500) NULL,
--	[overtime] [datetime] NULL,
--	[MCHID] [nvarchar](100) NULL,
--	[KEY] [nvarchar](100) NULL,
--	[NOTIFY_URL] [nvarchar](100) NULL,
--	[USER_NOTIFY_URL] [nvarchar](100) NULL,
--	[MCHIDKEY] [nvarchar](100) NULL,
--	[USERRegister_NOTIFY_URL] [nvarchar](100) NULL,
--PRIMARY KEY CLUSTERED 
--(
--	[setWeChatId] ASC
--)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
--) ON [PRIMARY]

--go
----drop table fz_Orders
--create table fz_Orders--订单
--(
--   id int identity(1,1) primary key,
--   userId int,--用户id
--   orderNo nvarchar(100),--流水号
--   amount decimal(18,2),--总金额
--   orderState int,--1 等待付款 2 已付款 
--   [type] int,--类型 1购票 2购卡 3充值卡
--   deductible decimal(18,2),--优惠卷抵扣金额
--   useScope nvarchar(200) default(''),--使用范围 为空不限制使用，否则绑定商户号,指定门店消费抵扣
--   payAmount decimal(18,2),--已支付的金额
--   payTime datetime,--支付时间
--   subTime datetime,--下单时间
--)
--go
--create table fz_OrderDetial--订单详情
--(
--   id int identity(1,1) primary key,
--   orderNo nvarchar(100),--流水号
--   amount decimal(18,2),--单价
--   sourceId int,--源Id
--   --[type] int,--类型 1购票 2购卡 3充值卡
--   num int,--数量
--   cardNo nvarchar(100),--卡号
--   subTime datetime,--下单时间
--)
--go
--create table UserCoupons--用户优惠卷
--(
--   id int identity(1,1) primary key,
--   couponsId int,--优惠卷id
--   userId int,--用户id
--   [state] int,--1未使用 2已使用 3已过期
--   receiveTime datetime,--领取时间
--   useTime datetime,--使用时间
--)
--go
----drop table UseCouponslog
--create table UseCouponslog--优惠卷使用记录
--(
--   id int identity(1,1) primary key,
--   userId int,--使用用户
--   couponsId int,--优惠卷id
--   orderNo nvarchar(100),--流水号
--   amount decimal(18,2),--金额
--   discount decimal(18,2),--折扣
--   --reduceAmount decimal(18,2),--满减金额
--   --deductibleAmount decimal(18,2),--抵扣金额
--   useTime datetime,--使用时间
--)

--GO
--create PROCEDURE [dbo].[P_getOrders]
-- @userId int=null,
-- @mobile nvarchar(50)=null,
-- @orderNo nvarchar(100)=null,
-- @orderState int=null,
-- @type int=null,
-- @pageIndex INT,
-- @pageSize INT
--AS
--BEGIN
--SELECT count(1) as Total from fz_Orders t left join Users u on t.userId=u.UserId where
--(@userId is null or t.UserId = @userId) 
--and (@mobile is null or u.Mobile = @mobile) 
--and (@orderNo is null or t.orderNo = @orderNo) 
--and (@orderState is null or t.orderState = @orderState) 
--and (@type is null or t.type = @type) 
-- select * from  (select Row_Number() OVER(order by t.subTime desc )AS RowNum , t.* from fz_Orders t left join Users u on t.userId=u.UserId where
--(@userId is null or t.UserId = @userId) 
--and (@mobile is null or u.Mobile = @mobile) 
--and (@orderNo is null or t.orderNo = @orderNo) 
--and (@orderState is null or t.orderState = @orderState) 
--and (@type is null or t.type = @type)
--)t
--where t.RowNum > (@pageIndex -1) * @pageSize 
--         AND t.RowNum <=   @pageIndex * @pageSize
--END
go
alter table Accounts add SaleAmount decimal(18,2)
alter table Accounts add TotalTimes int
alter table Accounts add SinglePrice decimal(18,2)
GO
ALTER PROCEDURE [dbo].[P_getAccounts]
 @State int=null,
 @Name nvarchar(50)=null,
 @ShopId int=null,
 @AccountToken nvarchar(100)=null,
 @States nvarchar(600)=null,
 @Ids nvarchar(600)=null,
 @AccountTypeId int=null,
 @IsMobileAvailable bit=null,
 @Content nvarchar(100)=null,
 @NameWith nvarchar(50)=null,
 @MobileWith nvarchar(50)=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from accounts t left join Users u on t.OwnerId=u.UserId where
(@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@ShopId is null or t.DistributorId=@ShopId)
and (@AccountToken is null or t.AccountToken = @AccountToken)
and (@States is null or t.State  in (@States))
and (@Ids is null or t.AccountId in (@Ids))
and (@NameWith is null or t.Name like '%'+ @NameWith + '%') 
and (@MobileWith is null or u.Mobile=@MobileWith) 
and (@AccountTypeId is null or t.AccountTypeId = @AccountTypeId) 
and (@IsMobileAvailable is null or 
	(@IsMobileAvailable = 1 and exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1)) or
	(@IsMobileAvailable = 0 and not exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1))
	) 
and	
(@Content is null or u.Mobile like '%'+@Content+ '%')

 select * from  (select Row_Number() OVER(order by t.Name )AS RowNum , t.*,u.DisplayName as 'OwnerDisplayName',u.Mobile as 'OwnerMobileNumber',u.babyBirthDate,u.babyName,u.babySex from accounts t left join Users u on t.OwnerId=u.UserId where
(@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@ShopId is null or t.DistributorId=@ShopId)
and (@AccountToken is null or t.AccountToken = @AccountToken)
and (@States is null or t.State  in (@States))
and (@Ids is null or t.AccountId in (@Ids))
and (@NameWith is null or t.Name like '%'+ @NameWith + '%') 
and (@AccountTypeId is null or t.AccountTypeId = @AccountTypeId) 
and (@MobileWith is null or u.Mobile=@MobileWith) 
and (@IsMobileAvailable is null or 
	(@IsMobileAvailable = 1 and exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1)) or
	(@IsMobileAvailable = 0 and not exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1))
	)
and	
(@Content is null or u.Mobile like '%'+@Content+ '%')
)t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END