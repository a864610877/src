--alter table users add babyName nvarchar(100)
--alter table users add babySex int
--alter table users add babyBirthDate datetime
--alter table users add openId nvarchar(200)

--drop table Tickets
--ʹ�÷�Χ Ϊ�ղ�����ʹ�ã�������̻���
--alter table accounts add useScope nvarchar(200) default('')
--create table Tickets
--(
--   id int identity(1,1) primary key,
--   userId int,
--   orderNo varchar(100),--������
--   AdmissionTicketId int,
--   Code nvarchar(200),
--   ExpiredDate datetime,
--   Price decimal(18,2),
--   adultNum int,
--   childNum int,
--   useScope nvarchar(200) default(''),--ʹ�÷�Χ Ϊ�ղ�����ʹ�ã�������̻���,ָ���ŵ�����
--   [State] int,--1 δʹ�� 2 ��ʹ�� 3 �ѹ���
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
--create table fz_Orders--����
--(
--   id int identity(1,1) primary key,
--   userId int,--�û�id
--   orderNo nvarchar(100),--��ˮ��
--   amount decimal(18,2),--�ܽ��
--   orderState int,--1 �ȴ����� 2 �Ѹ��� 
--   [type] int,--���� 1��Ʊ 2���� 3��ֵ��
--   deductible decimal(18,2),--�Żݾ�ֿ۽��
--   useScope nvarchar(200) default(''),--ʹ�÷�Χ Ϊ�ղ�����ʹ�ã�������̻���,ָ���ŵ����ѵֿ�
--   payAmount decimal(18,2),--��֧���Ľ��
--   payTime datetime,--֧��ʱ��
--   subTime datetime,--�µ�ʱ��
--)
--go
--create table fz_OrderDetial--��������
--(
--   id int identity(1,1) primary key,
--   orderNo nvarchar(100),--��ˮ��
--   amount decimal(18,2),--����
--   sourceId int,--ԴId
--   --[type] int,--���� 1��Ʊ 2���� 3��ֵ��
--   num int,--����
--   cardNo nvarchar(100),--����
--   subTime datetime,--�µ�ʱ��
--)
--go
--create table UserCoupons--�û��Żݾ�
--(
--   id int identity(1,1) primary key,
--   couponsId int,--�Żݾ�id
--   userId int,--�û�id
--   [state] int,--1δʹ�� 2��ʹ�� 3�ѹ���
--   receiveTime datetime,--��ȡʱ��
--   useTime datetime,--ʹ��ʱ��
--)
--go
----drop table UseCouponslog
--create table UseCouponslog--�Żݾ�ʹ�ü�¼
--(
--   id int identity(1,1) primary key,
--   userId int,--ʹ���û�
--   couponsId int,--�Żݾ�id
--   orderNo nvarchar(100),--��ˮ��
--   amount decimal(18,2),--���
--   discount decimal(18,2),--�ۿ�
--   --reduceAmount decimal(18,2),--�������
--   --deductibleAmount decimal(18,2),--�ֿ۽��
--   useTime datetime,--ʹ��ʱ��
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

go
create table ticketOff --��Ʊ������¼
(
   id int identity(1,1) primary key,
   userId int,--�û�id
   offType int,--���� 1 ��Ʊ 2��
   code nvarchar(100),--���� ��Ʊ����
   DisplayName nvarchar(100),--������/��Ʊ����
   shopId int,--�̻�
   timers int,--����
   offOp nvarchar(100),--����Ա
   subTime datetime,--����ʱ��
)
GO
create PROCEDURE [dbo].[P_getTicketOff]
 @type int=null,
 @mobile nvarchar(100)=null,
 @shopName nvarchar(100)=null,
 @shopDisplayName nvarchar(100)=null,
 @Bdate datetime=null,
 @Edate datetime=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from TicketOff t left join Shops s on t.shopId=s.ShopId left join Users u on t.userId=u.UserId where
(@type is null or t.offType = @type)
and (@mobile is null or  u.Mobile = @mobile)
and (@shopName is null or  s.Name = @shopName)
and (@shopDisplayName is null or  s.DisplayName like '%'+ @shopDisplayName+'%')
and (@Bdate is null or  t.subTime >= @Bdate)
and (@Edate is null or  t.subTime < @Edate)

 select * from  (select Row_Number() OVER(order by  t.subTime desc)AS RowNum , t.*,u.Mobile as 'mobile',u.DisplayName as 'userDisplayName',s.Name as 'shopName',s.DisplayName as 'shopDisplayName'
   from TicketOff t left join Shops s on t.shopId=s.ShopId left join Users u on t.userId=u.UserId where
(@type is null or t.offType = @type)
and (@mobile is null or  u.Mobile = @mobile)
and (@shopName is null or  s.Name = @shopName)
and (@shopDisplayName is null or  s.DisplayName like '%'+ @shopDisplayName+'%')
and (@Bdate is null or  t.subTime >= @Bdate)
and (@Edate is null or  t.subTime < @Edate)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END