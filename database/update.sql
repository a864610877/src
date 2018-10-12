--alter table users add babyName nvarchar(100)
--alter table users add babySex int
--alter table users add babyBirthDate datetime
--alter table users add openId nvarchar(200)

--drop table Tickets
--使用范围 为空不限制使用，否则绑定商户号
alter table accounts add useScope nvarchar(200) default('')
create table Tickets
(
   id int identity(1,1) primary key,
   userId int,
   orderNo varchar(100),--订单号
   AdmissionTicketId int,
   Code nvarchar(200),
   ExpiredDate datetime,
   Price decimal(18,2),
   adultNum int,
   childNum int,
   useScope nvarchar(200) default(''),--使用范围 为空不限制使用，否则绑定商户号,指定门店消费
   [State] int,--1 未使用 2 已使用 3 已过期
   BuyTime datetime
)

go

GO
create PROCEDURE [dbo].[P_getTickets]
 @userId int=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
(@userId is null or t.UserId = @userId)

 select * from  (select Row_Number() OVER(order by t.code )AS RowNum , t.*,u.name as 'TicketName' from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
(@userId is null or t.UserId = @userId)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO

CREATE TABLE [dbo].[SetWeChats](
	[setWeChatId] [int] IDENTITY(1,1) NOT NULL,
	[appID] [nvarchar](200) NULL,
	[appSecret] [nvarchar](200) NULL,
	[token] [nvarchar](200) NULL,
	[access_token] [nvarchar](500) NULL,
	[overtime] [datetime] NULL,
	[MCHID] [nvarchar](100) NULL,
	[KEY] [nvarchar](100) NULL,
	[NOTIFY_URL] [nvarchar](100) NULL,
	[USER_NOTIFY_URL] [nvarchar](100) NULL,
	[MCHIDKEY] [nvarchar](100) NULL,
	[USERRegister_NOTIFY_URL] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[setWeChatId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

go
--drop table fz_Orders
create table fz_Orders--订单
(
   id int identity(1,1) primary key,
   userId int,--用户id
   orderNo nvarchar(100),--流水号
   amount decimal(18,2),--总金额
   orderState int,--1 等待付款 2 已付款 
   [type] int,--类型 1购票 2购卡 3充值卡
   deductible decimal(18,2),--优惠卷抵扣金额
   useScope nvarchar(200) default(''),--使用范围 为空不限制使用，否则绑定商户号,指定门店消费抵扣
   payAmount decimal(18,2),--已支付的金额
   payTime datetime,--支付时间
   subTime datetime,--下单时间
)
go
create table fz_OrderDetial--订单详情
(
   id int identity(1,1) primary key,
   orderNo nvarchar(100),--流水号
   amount decimal(18,2),--单价
   sourceId int,--源Id
   --[type] int,--类型 1购票 2购卡 3充值卡
   num int,--数量
   cardNo nvarchar(100),--卡号
   subTime datetime,--下单时间
)
go
create table UserCoupons--用户优惠卷
(
   id int identity(1,1) primary key,
   couponsId int,--优惠卷id
   userId int,--用户id
   [state] int,--1未使用 2已使用 3已过期
   receiveTime datetime,--领取时间
   useTime datetime,--使用时间
)
go
--drop table UseCouponslog
create table UseCouponslog--优惠卷使用记录
(
   id int identity(1,1) primary key,
   userId int,--使用用户
   couponsId int,--优惠卷id
   orderNo nvarchar(100),--流水号
   amount decimal(18,2),--金额
   discount decimal(18,2),--折扣
   --reduceAmount decimal(18,2),--满减金额
   --deductibleAmount decimal(18,2),--抵扣金额
   useTime datetime,--使用时间
)



