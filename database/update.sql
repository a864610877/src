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
--go
--alter table Accounts add SaleAmount decimal(18,2)
--alter table Accounts add TotalTimes int
--alter table Accounts add SinglePrice decimal(18,2)
--GO
--ALTER PROCEDURE [dbo].[P_getAccounts]
-- @State int=null,
-- @Name nvarchar(50)=null,
-- @ShopId int=null,
-- @AccountToken nvarchar(100)=null,
-- @States nvarchar(600)=null,
-- @Ids nvarchar(600)=null,
-- @AccountTypeId int=null,
-- @IsMobileAvailable bit=null,
-- @Content nvarchar(100)=null,
-- @NameWith nvarchar(50)=null,
-- @MobileWith nvarchar(50)=null,
-- @pageIndex INT,
-- @pageSize INT
--AS
--BEGIN
--SELECT count(1) as Total from accounts t left join Users u on t.OwnerId=u.UserId where
--(@Name is null or t.Name = @Name)
--and (@State is null or t.State = @State)
--and (@ShopId is null or t.DistributorId=@ShopId)
--and (@AccountToken is null or t.AccountToken = @AccountToken)
--and (@States is null or t.State  in (@States))
--and (@Ids is null or t.AccountId in (@Ids))
--and (@NameWith is null or t.Name like '%'+ @NameWith + '%') 
--and (@MobileWith is null or u.Mobile=@MobileWith) 
--and (@AccountTypeId is null or t.AccountTypeId = @AccountTypeId) 
--and (@IsMobileAvailable is null or 
--	(@IsMobileAvailable = 1 and exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1)) or
--	(@IsMobileAvailable = 0 and not exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1))
--	) 
--and	
--(@Content is null or u.Mobile like '%'+@Content+ '%')

-- select * from  (select Row_Number() OVER(order by t.Name )AS RowNum , t.*,u.DisplayName as 'OwnerDisplayName',u.Mobile as 'OwnerMobileNumber',u.babyBirthDate,u.babyName,u.babySex from accounts t left join Users u on t.OwnerId=u.UserId where
--(@Name is null or t.Name = @Name)
--and (@State is null or t.State = @State)
--and (@ShopId is null or t.DistributorId=@ShopId)
--and (@AccountToken is null or t.AccountToken = @AccountToken)
--and (@States is null or t.State  in (@States))
--and (@Ids is null or t.AccountId in (@Ids))
--and (@NameWith is null or t.Name like '%'+ @NameWith + '%') 
--and (@AccountTypeId is null or t.AccountTypeId = @AccountTypeId) 
--and (@MobileWith is null or u.Mobile=@MobileWith) 
--and (@IsMobileAvailable is null or 
--	(@IsMobileAvailable = 1 and exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1)) or
--	(@IsMobileAvailable = 0 and not exists(select * from users u where t.ownerId = u.userId and u.IsMobileAvailable = 1))
--	)
--and	
--(@Content is null or u.Mobile like '%'+@Content+ '%')
--)t
--where t.RowNum >  (@pageIndex -1) * @pageSize 
--         AND t.RowNum <=   @pageIndex * @pageSize
--END

--go
--create table ticketOff --门票核销记录
--(
--   id int identity(1,1) primary key,
--   userId int,--用户id
--   offType int,--类型 1 门票 2卡
--   code nvarchar(100),--卡号 门票代码
--   DisplayName nvarchar(100),--卡名称/门票名称
--   shopId int,--商户
--   timers int,--次数
--   offOp nvarchar(100),--核销员
--   subTime datetime,--核销时间
--)
--GO
--create PROCEDURE [dbo].[P_getTicketOff]
-- @type int=null,
-- @mobile nvarchar(100)=null,
-- @shopName nvarchar(100)=null,
-- @shopDisplayName nvarchar(100)=null,
-- @Bdate datetime=null,
-- @Edate datetime=null,
-- @pageIndex INT,
-- @pageSize INT
--AS
--BEGIN
--SELECT count(1) as Total from TicketOff t left join Shops s on t.shopId=s.ShopId left join Users u on t.userId=u.UserId where
--(@type is null or t.offType = @type)
--and (@mobile is null or  u.Mobile = @mobile)
--and (@shopName is null or  s.Name = @shopName)
--and (@shopDisplayName is null or  s.DisplayName like '%'+ @shopDisplayName+'%')
--and (@Bdate is null or  t.subTime >= @Bdate)
--and (@Edate is null or  t.subTime < @Edate)

-- select * from  (select Row_Number() OVER(order by  t.subTime desc)AS RowNum , t.*,u.Mobile as 'mobile',u.DisplayName as 'userDisplayName',s.Name as 'shopName',s.DisplayName as 'shopDisplayName'
--   from TicketOff t left join Shops s on t.shopId=s.ShopId left join Users u on t.userId=u.UserId where
--(@type is null or t.offType = @type)
--and (@mobile is null or  u.Mobile = @mobile)
--and (@shopName is null or  s.Name = @shopName)
--and (@shopDisplayName is null or  s.DisplayName like '%'+ @shopDisplayName+'%')
--and (@Bdate is null or  t.subTime >= @Bdate)
--and (@Edate is null or  t.subTime < @Edate)
--)t
--where t.RowNum > (@pageIndex -1) * @pageSize 
--         AND t.RowNum <=   @pageIndex * @pageSize
--END
GO
ALTER PROCEDURE [dbo].[P_getOrders]
 @userId int=null,
 @mobile nvarchar(50)=null,
 @orderNo nvarchar(100)=null,
 @orderState int=null,
 @type int=null,
 @useScope nvarchar(100)=null,
 @Bdate datetime=null,
 @Edate datetime=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from fz_Orders t left join Users u on t.userId=u.UserId where
(@userId is null or t.UserId = @userId) 
and (@mobile is null or u.Mobile = @mobile) 
and (@orderNo is null or t.orderNo = @orderNo) 
and (@orderState is null or t.orderState = @orderState) 
and (@type is null or t.type = @type) 
and (@useScope is null or t.useScope = @useScope) 
and (@Bdate is null or t.subTime >= @Bdate)
and (@Edate is null or t.subTime < @Edate)
 select * from  (select Row_Number() OVER(order by t.subTime desc )AS RowNum , t.*,u.Mobile,u.DisplayName as userDisplayName,(select top 1 DisplayName from Shops where Name=t.useScope) as shopName from fz_Orders t left join Users u on t.userId=u.UserId where
(@userId is null or t.UserId = @userId) 
and (@mobile is null or u.Mobile = @mobile) 
and (@orderNo is null or t.orderNo = @orderNo) 
and (@orderState is null or t.orderState = @orderState) 
and (@type is null or t.type = @type)
and (@useScope is null or t.useScope = @useScope) 
and (@Bdate is null or t.subTime >= @Bdate)
and (@Edate is null or t.subTime < @Edate)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END

GO
alter PROCEDURE [dbo].[P_getTicketss]
@userId int=null,
@ticketName nvarchar(100)=null,
@orderNo nvarchar(100)=null,
@mobile nvarchar(100)=null,
@code nvarchar(100)=null,
@state int=null,
@useScope nvarchar(100)=null,
@Bdate datetime=null,
@Edate datetime=null,
@pageIndex INT,
@pageSize INT
AS
BEGIN
SELECT count(1) as Total from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id left join Users uu on t.userId=uu.UserId  where
(@userId is null or t.UserId = @userId)
and (@orderNo is null or t.orderNo = @orderNo)
and (@mobile is null or uu.Mobile = @mobile)
and (@ticketName is null or u.name like '%'+@ticketName+'%')
and (@code is null or t.Code = @code)
and (@state is null or t.State = @state)
and (@useScope is null or t.useScope = @useScope)
and (@Bdate is null or t.userTime >= @Bdate)
and (@Bdate is null or t.userTime < @Edate)

 select * from  (select Row_Number() OVER(order by t.code )AS RowNum , t.*,u.name as 'TicketName',u.introduce,uu.Mobile,uu.DisplayName as 'UserDisplayName',(select top 1 DisplayName from Shops where Name=t.useScope) as shopName from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id left join Users uu on t.userId=uu.UserId where
(@userId is null or t.UserId = @userId)
and (@orderNo is null or t.orderNo = @orderNo)
and (@mobile is null or uu.Mobile = @mobile)
and (@ticketName is null or u.name like '%'+@ticketName+'%')
and (@code is null or t.Code = @code)
and (@state is null or t.State = @state)
and (@useScope is null or t.useScope = @useScope)
and (@Bdate is null or t.userTime >= @Bdate)
and (@Bdate is null or t.userTime < @Edate)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
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
and (@ShopId is null or t.ShopId=@ShopId)
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

 select * from  (select Row_Number() OVER(order by t.Name )AS RowNum , t.*,u.DisplayName as 'OwnerDisplayName',u.Mobile as 'OwnerMobileNumber',u.babyBirthDate,u.babyName,u.babySex,(select DisplayName from Shops where Name=t.useScope) as shopName from accounts t left join Users u on t.OwnerId=u.UserId where
(@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@ShopId is null or t.ShopId=@ShopId)
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
go
create table WindowTicketings
(
   id int identity(1,1) primary key,
   code nvarchar(100),
   admissionTicketId int,
   shopId int,
   price decimal(18,2),--单价
   num int,--购买数量
   discount decimal(18,2),--折扣
   ticketName nvarchar(200),--门票名称
   amount decimal(18,2),--金额
   payType int,--支付方式 1 现金 2微信支付 3支付宝 4银联 5 其他
   displayName nvarchar(100),--姓名
   mobile nvarchar(100),--手机号
   babyName nvarchar(100),--宝宝姓名
   babySex int,--宝宝性别
   babyBirthDate datetime,--宝宝出生年月
   createTime datetime
)
go
alter PROCEDURE [dbo].[P_getWindowTicketings]
 @shopId int=null,
 @admissionTicketId int=null,
 @mobile nvarchar(100)=null,
 @shopName nvarchar(100)=null,
 @payType int=null,
 @Bdate datetime=null,
 @Edate datetime=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from WindowTicketings t left join Shops s on t.shopId=s.ShopId where
(@admissionTicketId is null or t.admissionTicketId = @admissionTicketId)
and (@shopId is null or  t.shopId = @shopId)
and (@mobile is null or  t.mobile = @mobile)
and (@shopName is null or  s.Name = @shopName)
and (@payType is null or  t.payType=@payType)
and (@Bdate is null or  t.createTime >= @Bdate)
and (@Edate is null or  t.createTime < @Edate)

 select * from  (select Row_Number() OVER(order by  t.createTime desc)AS RowNum , t.*,s.Name as 'shopName',s.DisplayName as 'shopDisplayName'
   from WindowTicketings t left join Shops s on t.shopId=s.ShopId where
(@admissionTicketId is null or t.admissionTicketId = @admissionTicketId)
and (@shopId is null or  t.shopId = @shopId)
and (@mobile is null or  t.mobile = @mobile)
and (@shopName is null or  s.Name = @shopName)
and (@payType is null or  t.payType=@payType)
and (@Bdate is null or  t.createTime >= @Bdate)
and (@Edate is null or  t.createTime < @Edate)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
go
create table PostToken
(
   id int identity(1,1) primary key,
   posName nvarchar(100), --终端号
   token nvarchar(200),--密钥
   createTime datetime,--创建时间
)

go
create table HandRingPrint--手环打印列表
(
   id int identity(1,1) primary key,
   shopId int,--核销门店
   code nvarchar(100),--票号/卡号
   ticketType int,--1门票 2卡
   adultNum int,--大人数量
   childNum int,--小孩数量
   userName nvarchar(100),--姓名
   mobile nvarchar(50),--电话
   babyName nvarchar(100),--小孩姓名
   babySex nvarchar(100),--小孩性别
   babyBirthDate nvarchar(100),--小孩出生年月
   state int,--状态 1未打印 2已打印
   printTime datetime,--打印时间
   createTime datetime
)
go
alter PROCEDURE [dbo].[P_getHandRingPrint]
 @shopId int=null,
 @code nvarchar(100)=null,
 @mobile nvarchar(100)=null,
 @babyName nvarchar(100)=null,
 @ticketType int=null,
 @state int=null,
 @Bdate datetime=null,
 @Edate datetime=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from HandRingPrint t  where
(@code is null or t.code like '%'+@code+'%')
and (@shopId is null or  t.shopId = @shopId)
and (@mobile is null or  t.mobile = @mobile)
and (@babyName is null or  t.babyName = @babyName)
and (@ticketType is null or  t.ticketType=@ticketType)
and (@state is null or  t.state=@state)
and (@Bdate is null or  t.createTime >= @Bdate)
and (@Edate is null or  t.createTime < @Edate)

 select * from  (select Row_Number() OVER(order by  t.createTime desc)AS RowNum , t.*
   from HandRingPrint t where
(@code is null or t.code like '%'+@code+'%')
and (@shopId is null or  t.shopId = @shopId)
and (@mobile is null or  t.mobile = @mobile)
and (@babyName is null or  t.babyName = @babyName)
and (@ticketType is null or  t.ticketType=@ticketType)
and (@state is null or  t.state=@state)
and (@Bdate is null or  t.createTime >= @Bdate)
and (@Edate is null or  t.createTime < @Edate)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
