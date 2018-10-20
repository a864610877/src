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

GO
create PROCEDURE [dbo].[P_getOrders]
 @userId int=null,
 @mobile nvarchar(50)=null,
 @orderNo nvarchar(100)=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from fz_Orders t left join Users u on t.userId=u.UserId where
(@userId is null or t.UserId = @userId)
(@mobile is null or u.Mobile = @userId)

 select * from  (select Row_Number() OVER(order by t.code )AS RowNum , t.*,u.name as 'TicketName' from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
(@userId is null or t.UserId = @userId)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END

