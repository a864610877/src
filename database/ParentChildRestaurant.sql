USE [ParentChildRestaurant]
GO
/****** Object:  StoredProcedure [dbo].[CreateDealLogDaily]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[CreateDealLogDaily]
				@start datetime
			AS
			BEGIN  
				declare @end datetime
				set @end = dateadd(day, 1, @start)
				delete ReportDealLogDay where submitdate = @start
				insert ReportDealLogDay(dealtype,accountTypeId,amount,point,[Count],[SubmitDate])
					select dealtype, acc.accountTypeId, isnull(sum(t.Amount), 0) amount, isnull(sum(t.Point), 0) point, isnull(count(t.amount), 0) [count], @start SubmitDate from deallogs t
					inner join accounts acc on acc.accountid = t.accountid
					where (t.state = 1 or t.state = 4)  and t.submittime >= @start and t.submittime < @end
					group by dealtype, acc.accountTypeId
			END


GO
/****** Object:  StoredProcedure [dbo].[P_getAccounts]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getAccounts]
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

 select * from  (select Row_Number() OVER(order by t.Name )AS RowNum , t.*,u.DisplayName as 'OwnerDisplayName',u.Mobile as 'OwnerMobileNumber',u.babyBirthDate,u.babyName,u.babySex,(select DisplayName from Shops where Name=t.useScope) as shopName from accounts t left join Users u on t.OwnerId=u.UserId where
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
GO
/****** Object:  StoredProcedure [dbo].[P_getAdmissionTickets]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getAdmissionTickets]
 @state int=null,
 @name nvarchar(50)=null,
 @startTime datetime=null,
 @endTime datetime=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from AdmissionTicket t  where
(@name is null or t.name like '%'+@name+ '%')
and (@State is null or t.state = @state)
and (@startTime is null or t.crateTime>=@startTime)
and (@endTime is null or t.crateTime<@endTime) 

 select * from  (select Row_Number() OVER(order by t.crateTime)AS RowNum , t.* from AdmissionTicket t where
(@name is null or t.name like '%'+@name+ '%')
and (@State is null or t.state = @state)
and (@startTime is null or t.crateTime>=@startTime)
and (@endTime is null or t.crateTime<@endTime) 
)t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
GO
/****** Object:  StoredProcedure [dbo].[P_getArticles]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getArticles]  --获取文章列表

   @title nvarchar(50)=null,
   @pageIndex int,
   @pageSize int
   AS
   BEGIN
 SELECT count(1) as Total from fz_Articles t   where   
(@title is null or title LIKE '%'+@title+'%')  

	select *
	from  (select Row_Number() OVER(order by SubmitTime desc)AS RowNum, t.* from fz_Articles t where  
(
(@title is null or title LIKE '%'+@title+'%')
))t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  StoredProcedure [dbo].[P_getCoupons]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[P_getCoupons]
 @state int=null,
 @couponsType int=null,
 @name nvarchar(100)=null,
 @code nvarchar(100)=null,
 @startTime datetime=null,
 @endTime datetime=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from Coupons t  where
(@name is null or t.name like '%'+@name+ '%')
and (@code is null or t.code = @code)
and (@state is null or t.state = @state)
and (@couponsType is null or t.couponsType = @couponsType)
and (@startTime is null or t.createTime>=@startTime)
and (@endTime is null or t.createTime<@endTime) 

 select * from  (select Row_Number() OVER(order by t.createTime)AS RowNum , t.* from Coupons t where
(@name is null or t.name like '%'+@name+ '%')
and (@code is null or t.code = @code)
and (@state is null or t.state = @state)
and (@couponsType is null or t.couponsType = @couponsType)
and (@startTime is null or t.createTime>=@startTime)
and (@endTime is null or t.createTime<@endTime) 
)t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
GO
/****** Object:  StoredProcedure [dbo].[P_getDistributors]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[P_getDistributors]

AS
BEGIN
 SELECT count(1) as Total from  distributors 


	select * from  (select Row_Number() OVER(order by DistributorId )AS RowNum ,t.* from distributors t  ) t
--where t.RowNum >  (@pageIndex -1) * @pageSize 
--         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  StoredProcedure [dbo].[P_getLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getLogs]
@LogType int=null,
@ContentWith nvarchar(2000)=null,
@pageIndex INT,
@pageSize INT,
@UserName nvarchar(100)=null
AS
BEGIN

 SELECT count(1) as Total from logs t where  
(@LogType is null or LogType = @LogType)  
and (@ContentWith is null or Content like '%' + @ContentWith + '%')  
and (@UserName is null or UserName = @UserName)

	select * from  (select Row_Number() OVER(order by SubmitTime desc)AS RowNum, t.* from logs t where  
(@LogType is null or LogType = @LogType)  
and (@ContentWith is null or Content like '%' + @ContentWith + '%')  
and (@UserName is null or UserName = @UserName))t 
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  StoredProcedure [dbo].[P_getOrders]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
create PROCEDURE [dbo].[P_getOrders]
 @userId int=null,
 @mobile nvarchar(50)=null,
 @orderNo nvarchar(100)=null,
 @orderState int=null,
 @type int=null,
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
 select * from  (select Row_Number() OVER(order by t.subTime desc )AS RowNum , t.* from fz_Orders t left join Users u on t.userId=u.UserId where
(@userId is null or t.UserId = @userId) 
and (@mobile is null or u.Mobile = @mobile) 
and (@orderNo is null or t.orderNo = @orderNo) 
and (@orderState is null or t.orderState = @orderState) 
and (@type is null or t.type = @type)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
GO
/****** Object:  StoredProcedure [dbo].[P_getPosEndPoints]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getPosEndPoints]

	 @Name nvarchar(100)=null,
	 @pageIndex INT,
	 @pageSize INT,
	 @NameWith nvarchar(100)=null,
	 @State int=null,
	 @ShopId int=null
AS
BEGIN
SELECT count(1) as Total from  PosEndPoints t where 
(@State is null or t.State = @State)
and (@ShopId is null or t.ShopId = @ShopId) 
and (@Name is null or t.Name = @Name) 
and (@NameWith is null or t.Name Like '%' + @NameWith + '%' ) 

 select * from  (select Row_Number() OVER(order by PosEndPointId )AS RowNum, t.* from PosEndPoints t where 
(@State is null or t.State = @State)
and (@ShopId is null or t.ShopId = @ShopId) 
and (@Name is null or t.Name = @Name) 
and (@NameWith is null or t.Name Like '%' + @NameWith + '%' ) )t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  StoredProcedure [dbo].[P_getRoles]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getRoles]

	 @Name nvarchar(100)=null,
	 @pageIndex INT,
	 @pageSize INT,
	 @DisplayNameWith nvarchar(100)=null,
	 @State int=null,
	 @UserId int=null,
	 @NameWith nvarchar(100)=null
AS
BEGIN
   
SELECT count(1) as Total from  roles  where 
(@UserId is null or roles.RoleId in (Select ur.Role_RoleId from UserRoles ur where ur.User_UserId = @UserId))
and (@Name is null or @Name = Name)
and (@NameWith is null or Name like @NameWith)
and (@DisplayNameWith is null or DisplayName like @DisplayNameWith)
and (@State is null or State = @State)

	select * from  (select Row_Number() OVER(order by RoleId )AS RowNum ,t.* from roles t where 
(@UserId is null or t.RoleId in (Select ur.Role_RoleId from UserRoles ur where ur.User_UserId = @UserId))
and (@Name is null or @Name = Name)
and (@NameWith is null or Name like @NameWith)
and (@DisplayNameWith is null or DisplayName like @DisplayNameWith)
and (@State is null or State = @State)) t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  StoredProcedure [dbo].[P_getShops]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE  [dbo].[P_getShops]
@ContentWith nvarchar(2000)=null,
@pageIndex INT,
@pageSize INT,
@State int=null,
@IsBuildIn bit=null,
@IsMobileAvailable bit=null,
@ShopId int=null,
@Name nvarchar(100)=null,
@NameWith nvarchar(100)=null,
@DisplayNameWith nvarchar(100)=null,
@ShopIds varchar(100)=null
AS
BEGIN
	select COUNT(1) as Total from shops t where
(@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@IsBuildIn is null or t.BuildIn = @IsBuildIn)
and (@IsMobileAvailable is null 
        or ( @IsMobileAvailable = 1 and exists (select * from users u where u.shopId = t.shopId and u.IsMobileAvailable = 1))
        or ( @IsMobileAvailable = 0  and  not exists (select * from users u where u.shopId = t.shopId and u.IsMobileAvailable = 1))
    )
and (@ShopId is null or t.ShopId = @ShopId)
and (@NameWith is null or t.Name like '%'+ @NameWith + '%')
and (@DisplayNameWith is null or t.DisplayName like '%'+ @DisplayNameWith + '%')
and (@ShopIds is null or t.ShopId in (@ShopIds))

select * from(select ROW_NUMBER() over(order by ShopId) as RowNum,  t.* from shops t where
(@Name is null or t.Name = @Name)
and (@State is null or t.State = @State)
and (@IsBuildIn is null or t.BuildIn = @IsBuildIn)
and (@IsMobileAvailable is null 
        or ( @IsMobileAvailable = 1 and exists (select * from users u where u.shopId = t.shopId and u.IsMobileAvailable = 1))
        or ( @IsMobileAvailable = 0  and  not exists (select * from users u where u.shopId = t.shopId and u.IsMobileAvailable = 1))
    )
and (@ShopId is null or t.ShopId = @ShopId)
and (@NameWith is null or t.Name like '%'+ @NameWith + '%')
and (@DisplayNameWith is null or t.DisplayName like '%'+ @DisplayNameWith + '%')
and (@ShopIds is null or t.ShopId in (@ShopIds))) t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  StoredProcedure [dbo].[P_getTickets]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getTickets]
 @userId int=null,
 @pageIndex INT,
 @pageSize INT
AS
BEGIN
SELECT count(1) as Total from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
(@userId is null or t.UserId = @userId)

 select * from  (select Row_Number() OVER(order by t.code )AS RowNum , t.*,u.name as 'TicketName',u.introduce from Tickets t left join AdmissionTicket u on t.AdmissionTicketId=u.id where
(@userId is null or t.UserId = @userId)
)t
where t.RowNum > (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END
GO
/****** Object:  StoredProcedure [dbo].[P_getUsers]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE PROCEDURE [dbo].[P_getUsers]
 @RoleId int=null,
 @State int=null,
 @Name nvarchar(50)=null,
 @NameWith nvarchar(50)=null,
 @pageIndex INT,
 @pageSize INT,
 @DisplayNameWith nvarchar(50)=null,
 @EmailWith nvarchar(50)=null
 --@UserIds int,
 --@Discriminator nvarchar(128)
AS
BEGIN
SELECT count(1) as Total from  users t  where 
(@RoleId is null or t.UserId in (Select ur.User_UserId from UserRoles ur where ur.Role_RoleId = @RoleId))
and (@State is null or t.State = @State)
and (@Name is null or t.Name = @Name)
and (@NameWith is null or t.Name like '%' + @NameWith + '%')
and (@DisplayNameWith is null or t.DisplayName like '%' + @DisplayNameWith + '%')
and (@EmailWith is null or t.Email like '%' + @EmailWith + '%')
--and (@UserIds is null or t.UserId in (@UserIds))
and  t.Discriminator = 'AdminUser'

 select * from  (select Row_Number() OVER(order by t.UserId )AS RowNum , t.* from users t where 
(@RoleId is null or t.UserId in (Select ur.User_UserId from UserRoles ur where ur.Role_RoleId = @RoleId))
and (@State is null or t.State = @State)
and (@Name is null or t.Name = @Name)
and (@NameWith is null or t.Name like '%' + @NameWith + '%')
and (@DisplayNameWith is null or t.DisplayName like '%' + @DisplayNameWith + '%')
and (@EmailWith is null or t.Email like '%' + @EmailWith + '%')
--and (@UserIds is null or t.UserId in (@UserIds))
and  t.Discriminator = 'AdminUser'
)t
where t.RowNum >  (@pageIndex -1) * @pageSize 
         AND t.RowNum <=   @pageIndex * @pageSize
END


GO
/****** Object:  Table [dbo].[AccountLevelPointGifts]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountLevelPointGifts](
	[AccountLevel] [int] NOT NULL,
	[PointGiftId] [int] NOT NULL,
	[AccountTypeId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountLevel] ASC,
	[PointGiftId] ASC,
	[AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountLevelPointRebates]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountLevelPointRebates](
	[AccountLevel] [int] NOT NULL,
	[PointRebateId] [int] NOT NULL,
	[AccountTypeId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountLevel] ASC,
	[PointRebateId] ASC,
	[AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountLevelPolicies]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountLevelPolicies](
	[AccountLevelPolicyId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[TotalPointStart] [decimal](18, 2) NOT NULL,
	[Level] [int] NOT NULL,
	[State] [int] NOT NULL,
	[AccountTypeId] [int] NULL,
	[DiscountRate] [decimal](18, 2) NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountLevelPolicyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Accounts]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Accounts](
	[AccountId] [int] IDENTITY(1,1) NOT NULL,
	[OwnerId] [int] NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[FreezeAmount] [decimal](18, 2) NOT NULL,
	[ExpiredMonths] [int] NOT NULL,
	[ExpiredDate] [datetime] NOT NULL,
	[LastDealTime] [datetime] NOT NULL,
	[State] [int] NOT NULL,
	[AccountTypeId] [int] NOT NULL,
	[ShopId] [int] NOT NULL,
	[PayAmount] [decimal](18, 2) NOT NULL,
	[DepositAmount] [decimal](18, 2) NOT NULL,
	[AccountToken] [nvarchar](100) NULL,
	[Point] [int] NOT NULL,
	[TotalPoint] [int] NOT NULL,
	[AccountLevel] [int] NOT NULL,
	[RecordVersion] [int] NOT NULL,
	[AccountLevelName] [nvarchar](100) NULL,
	[Name] [nvarchar](100) NULL,
	[Password] [nvarchar](100) NULL,
	[InitPassword] [nvarchar](100) NULL,
	[PasswordSalt] [nvarchar](100) NULL,
	[OpenTime] [datetime] NULL,
	[IsMessageOfDeal] [bit] NULL,
	[SalerId] [int] NULL,
	[Remark1] [nvarchar](4000) NULL,
	[ChargingAmount] [decimal](18, 2) NULL,
	[limiteAmount] [decimal](18, 2) NULL,
	[DistributorId] [int] NULL,
	[IsRecharging] [int] NULL,
	[frequency] [int] NULL,
	[frequencyUsed] [int] NULL,
	[useScope] [nvarchar](200) NULL,
	[SaleAmount] [decimal](18, 2) NULL,
	[TotalTimes] [int] NULL,
	[SinglePrice] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AccountTypes]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AccountTypes](
	[AccountTypeId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[IsRecharging] [bit] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Point] [int] NOT NULL,
	[ExpiredMonths] [int] NOT NULL,
	[RenewMonths] [int] NOT NULL,
	[IsRenew] [bit] NOT NULL,
	[DepositAmount] [decimal](18, 2) NOT NULL,
	[State] [int] NOT NULL,
	[IsMessageOfDeal] [bit] NULL,
	[IsPointable] [bit] NULL,
	[IsSmsDeal] [bit] NULL,
	[IsSmsAccountBirthday] [bit] NULL,
	[IsSmsRecharge] [bit] NULL,
	[IsSmsTransfer] [bit] NULL,
	[IsSmsSuspend] [bit] NULL,
	[IsSmsResume] [bit] NULL,
	[IsSmsRenew] [bit] NULL,
	[IsSmsClose] [bit] NULL,
	[IsSmsMobile] [bit] NULL,
	[IsSmscancelMobile] [bit] NULL,
	[IsSmsCode] [bit] NULL,
	[IsSmsChangeName] [bit] NULL,
	[frequency] [int] NULL,
	[describe] [nvarchar](max) NULL,
	[numberOfPeople] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[AccountTypeId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AdmissionTicket]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AdmissionTicket](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[name] [nvarchar](200) NULL,
	[adultNum] [int] NULL,
	[childNum] [int] NULL,
	[addAdultAmount] [decimal](18, 2) NULL,
	[amount] [decimal](18, 2) NULL,
	[weekendAmount] [decimal](18, 2) NULL,
	[state] [int] NULL,
	[introduce] [nvarchar](500) NULL,
	[details] [nvarchar](max) NULL,
	[crateTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[AmountRates]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[AmountRates](
	[AmountRateId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[AccountLevel] [int] NOT NULL,
	[Days] [int] NOT NULL,
	[Rate] [decimal](18, 2) NULL,
	[Amount] [decimal](18, 2) NULL,
	[State] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[AmountRateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[CashDealLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[CashDealLogs](
	[CashDealLogId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[UserId] [int] NOT NULL,
	[OwnerId] [int] NOT NULL,
	[RecordVersion] [int] NOT NULL,
	[DealType] [int] NOT NULL,
	[State] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CashDealLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Commodities]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Commodities](
	[CommodityId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](128) NULL,
	[DisplayName] [nvarchar](128) NULL,
	[Price] [decimal](18, 2) NOT NULL,
	[State] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[CommodityId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Coupons]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Coupons](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[code] [nvarchar](100) NULL,
	[couponsType] [int] NULL,
	[name] [nvarchar](100) NULL,
	[useScope] [nvarchar](100) NULL,
	[discount] [decimal](18, 2) NULL,
	[deductibleAmount] [decimal](18, 2) NULL,
	[fullAmount] [decimal](18, 2) NULL,
	[reduceAmount] [decimal](18, 2) NULL,
	[quantity] [int] NULL,
	[validity] [datetime] NULL,
	[leadersOfNum] [int] NULL,
	[state] [int] NULL,
	[createOp] [nvarchar](100) NULL,
	[createTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DealLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DealLogs](
	[DealLogId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[SourceShopId] [int] NOT NULL,
	[SourceShopName] [nvarchar](100) NULL,
	[SourcePosId] [int] NOT NULL,
	[SourcePosName] [nvarchar](100) NULL,
	[State] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[AccountName] [nvarchar](100) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[SerialNo] [nvarchar](100) NULL,
	[DealType] [int] NOT NULL,
	[SourceShopDisplayName] [nvarchar](100) NULL,
	[AccountAmount] [decimal](18, 2) NOT NULL,
	[Addin] [int] NOT NULL,
	[Point] [int] NOT NULL,
	[IsHidden] [bit] NULL,
	[shopId] [int] NULL,
	[shopName] [nvarchar](50) NULL,
	[shopDisplayName] [nvarchar](50) NULL,
	[LiquidateDealLogId] [int] NOT NULL,
	[YAmount] [decimal](18, 2) NULL,
	[DiscountRate] [decimal](18, 2) NULL,
PRIMARY KEY CLUSTERED 
(
	[DealLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DealWays]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DealWays](
	[DealWayId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NOT NULL,
	[ApplyTo] [int] NOT NULL,
	[State] [int] NOT NULL,
	[isCash] [bit] NULL,
 CONSTRAINT [PK_DealWays] PRIMARY KEY CLUSTERED 
(
	[DealWayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DistributorAccountLevelPolicyRates]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DistributorAccountLevelPolicyRates](
	[DistributorId] [int] NOT NULL,
	[AccountLevelPolicyId] [int] NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_DistributorAccountLevelPolicyRates] PRIMARY KEY CLUSTERED 
(
	[DistributorId] ASC,
	[AccountLevelPolicyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[DistributorBrokerage]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[DistributorBrokerage](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[Bdate] [datetime] NOT NULL,
	[Edate] [datetime] NOT NULL,
	[DistributorId] [int] NOT NULL,
	[settlementDistributorId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[consume] [decimal](18, 2) NOT NULL,
	[Rate] [decimal](18, 2) NOT NULL,
	[brokerage] [decimal](18, 2) NOT NULL,
	[status] [bit] NOT NULL,
 CONSTRAINT [PK_DistributorBrokerage] PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Distributors]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Distributors](
	[DistributorId] [int] IDENTITY(1,1) NOT NULL,
	[UserId] [int] NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[Address] [nvarchar](200) NULL,
	[ParentId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[RecordVersion] [int] NULL,
	[DistributorLevel] [int] NULL,
	[DealWayId] [int] NULL,
	[Bank] [varchar](100) NULL,
	[Description] [varchar](100) NULL,
	[BankPoint] [varchar](100) NULL,
	[BankAccountName] [varchar](100) NULL,
	[BankUserName] [varchar](100) NULL,
	[Name] [varchar](100) NULL,
	[DisplayName] [varchar](100) NULL,
 CONSTRAINT [PK_Distributors] PRIMARY KEY CLUSTERED 
(
	[DistributorId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[EdmMetadata]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[EdmMetadata](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[ModelHash] [nvarchar](max) NULL,
PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ErrorLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ErrorLogs](
	[ErrorLogId] [int] IDENTITY(1,1) NOT NULL,
	[Title] [nvarchar](100) NOT NULL,
	[Error] [nvarchar](4000) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[ErrorLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fz_Articles]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fz_Articles](
	[articleId] [int] IDENTITY(1,1) NOT NULL,
	[title] [nvarchar](100) NULL,
	[imageUrl] [nvarchar](100) NULL,
	[describe] [nvarchar](max) NULL,
	[submitTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[articleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fz_OrderDetial]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fz_OrderDetial](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[orderNo] [nvarchar](100) NULL,
	[amount] [decimal](18, 2) NULL,
	[sourceId] [int] NULL,
	[num] [int] NULL,
	[cardNo] [nvarchar](100) NULL,
	[subTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[fz_Orders]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[fz_Orders](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NULL,
	[orderNo] [nvarchar](100) NULL,
	[amount] [decimal](18, 2) NULL,
	[orderState] [int] NULL,
	[type] [int] NULL,
	[deductible] [decimal](18, 2) NULL,
	[useScope] [nvarchar](200) NULL,
	[payAmount] [decimal](18, 2) NULL,
	[payTime] [datetime] NULL,
	[subTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Goods]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Goods](
	[GoodId] [int] IDENTITY(1,1) NOT NULL,
	[GoodName] [nvarchar](50) NOT NULL,
	[State] [int] NOT NULL,
 CONSTRAINT [PK_Goods] PRIMARY KEY CLUSTERED 
(
	[GoodId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Liquidates]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Liquidates](
	[LiquidateId] [int] IDENTITY(1,1) NOT NULL,
	[ShopId] [int] NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[DealAmount] [decimal](18, 2) NOT NULL,
	[CancelAmount] [decimal](18, 2) NOT NULL,
	[DealLogId] [int] NULL,
	[dealIds] [nvarchar](max) NULL,
	[state] [int] NULL,
	[Count] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[LiquidateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Logs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Logs](
	[LogId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Title] [nvarchar](100) NULL,
	[Content] [nvarchar](2000) NULL,
	[LogType] [int] NOT NULL,
	[Addin] [int] NULL,
	[SerialNo] [nvarchar](1000) NULL,
PRIMARY KEY CLUSTERED 
(
	[LogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[OrderDetial]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[OrderDetial](
	[Serialnumber] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [nvarchar](50) NOT NULL,
	[GoodId] [int] NOT NULL,
	[Amount] [int] NOT NULL,
	[price] [decimal](18, 2) NOT NULL,
 CONSTRAINT [PK_OrderDetial] PRIMARY KEY CLUSTERED 
(
	[Serialnumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Orders]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Orders](
	[Serialnumber] [int] IDENTITY(1,1) NOT NULL,
	[OrderId] [nvarchar](50) NOT NULL,
	[AccountId] [int] NOT NULL,
	[Address] [nvarchar](200) NULL,
	[TotalMoney] [decimal](18, 2) NOT NULL,
	[Creater] [nvarchar](50) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[State] [int] NOT NULL,
	[createDate] [datetime] NULL,
	[senderId] [int] NULL,
	[CreaterId] [int] NULL,
	[sender] [nvarchar](50) NULL,
	[Phone] [nvarchar](50) NULL,
	[Demo] [nvarchar](50) NULL,
 CONSTRAINT [PK_Orders] PRIMARY KEY CLUSTERED 
(
	[Serialnumber] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointGifts]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointGifts](
	[PointGiftId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[Point] [int] NOT NULL,
	[Photo] [nvarchar](1024) NULL,
	[Category] [nvarchar](128) NULL,
	[Description] [nvarchar](1024) NOT NULL,
	[State] [int] NOT NULL,
	[DependencyType] [int] NULL,
	[WeekDays] [nvarchar](4000) NULL,
	[AccountLevels] [nvarchar](4000) NULL,
	[Days] [nvarchar](4000) NULL,
	[Priority] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PointGiftId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointPolicies]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointPolicies](
	[PointPolicyId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[State] [int] NOT NULL,
	[Point] [decimal](18, 2) NOT NULL,
	[DependencyType] [int] NULL,
	[WeekDays] [nvarchar](4000) NULL,
	[AccountLevels] [nvarchar](4000) NULL,
	[Days] [nvarchar](4000) NULL,
	[Priority] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[PointPolicyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointRebateLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointRebateLogs](
	[PointRebateLogId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Point] [int] NOT NULL,
	[UserId] [int] NOT NULL,
	[AccountId] [int] NOT NULL,
	[PointRebateId] [int] NOT NULL,
	[SerialNo] [nvarchar](50) NOT NULL
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PointRebates]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PointRebates](
	[PointRebateId] [int] IDENTITY(1,1) NOT NULL,
	[DisplayName] [nvarchar](100) NULL,
	[Point] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[State] [int] NOT NULL,
	[DependencyType] [int] NULL,
	[WeekDays] [nvarchar](4000) NULL,
	[AccountLevels] [nvarchar](4000) NULL,
	[Days] [nvarchar](4000) NULL,
PRIMARY KEY CLUSTERED 
(
	[PointRebateId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PosEndPoints]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PosEndPoints](
	[PosEndPointId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DisplayName] [nvarchar](100) NULL,
	[TokenKey] [nvarchar](100) NULL,
	[ShopId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[DataKey] [nvarchar](50) NULL,
	[CurrentUserId] [int] NULL,
	[RecordVersion] [int] NULL,
	[IsReimburse] [bit] NULL,
	[IsPay] [bit] NULL,
	[IsRecharge] [bit] NULL,
PRIMARY KEY CLUSTERED 
(
	[PosEndPointId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PosKey]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PosKey](
	[PosKeyId] [int] IDENTITY(1,1) NOT NULL,
	[ShopName] [nvarchar](50) NULL,
	[PosName] [nvarchar](50) NULL,
	[Key1] [nvarchar](100) NULL,
	[Key2] [nvarchar](100) NULL,
PRIMARY KEY CLUSTERED 
(
	[PosKeyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PrePays]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PrePays](
	[PrePayId] [int] IDENTITY(1,1) NOT NULL,
	[SerialNo] [nvarchar](100) NULL,
	[SubmitTime] [datetime] NOT NULL,
	[ShopId] [int] NOT NULL,
	[PosId] [int] NOT NULL,
	[ShopName] [nvarchar](100) NULL,
	[ShopDisplayName] [nvarchar](100) NULL,
	[AccountId] [int] NOT NULL,
	[AccountName] [nvarchar](100) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[ActualAmount] [decimal](18, 2) NOT NULL,
	[State] [int] NOT NULL,
	[updatetime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[PrePayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[PrintTickets]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[PrintTickets](
	[PrintTicketId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[LogType] [int] NULL,
	[AccountName] [nvarchar](50) NOT NULL,
	[Content] [nvarchar](4000) NOT NULL,
	[AccountId] [int] NULL,
	[PrintCount] [int] NOT NULL,
	[SerialNo] [nvarchar](1000) NULL,
 CONSTRAINT [PK_PrintTickets] PRIMARY KEY CLUSTERED 
(
	[PrintTicketId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RechargingLog]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RechargingLog](
	[RechargingLogId] [int] IDENTITY(1,1) NOT NULL,
	[AccountAmount] [decimal](18, 2) NOT NULL,
	[Name] [nvarchar](50) NULL,
	[RechargingAmount] [decimal](18, 2) NOT NULL,
	[RechargAccountAmount] [decimal](18, 2) NOT NULL,
	[Decs] [nvarchar](500) NULL,
	[State] [int] NULL,
	[SubmitTime] [datetime] NULL,
	[AccountName] [nvarchar](50) NOT NULL,
	[serialNoAll] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_RechargingLog] PRIMARY KEY CLUSTERED 
(
	[RechargingLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportAccountDeals]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportAccountDeals](
	[AccountDealsReportId] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[AccountName] [nvarchar](50) NOT NULL,
	[PayAmount] [decimal](18, 2) NOT NULL,
	[PayCount] [int] NOT NULL,
	[DonePrepayAmount] [decimal](18, 2) NOT NULL,
	[DonePrepayCount] [int] NOT NULL,
	[CancelAmount] [decimal](18, 2) NOT NULL,
	[CancelCount] [int] NOT NULL,
	[CancelDonePrepayAmount] [decimal](18, 2) NOT NULL,
	[CancelDonePrepayCount] [int] NOT NULL,
	[UnPayCount] [int] NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
 CONSTRAINT [PK_AccountDealsReport] PRIMARY KEY CLUSTERED 
(
	[AccountDealsReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportAccountMonth]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportAccountMonth](
	[AccountMonthtId] [int] IDENTITY(1,1) NOT NULL,
	[AccountId] [int] NOT NULL,
	[AccountName] [nvarchar](50) NOT NULL,
	[DealAmount] [decimal](18, 2) NOT NULL,
	[CancelAmount] [decimal](18, 2) NOT NULL,
	[Month] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_AccountMonthReport] PRIMARY KEY CLUSTERED 
(
	[AccountMonthtId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportDealLogDay]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportDealLogDay](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[dealtype] [int] NOT NULL,
	[accountTypeId] [int] NOT NULL,
	[amount] [decimal](18, 2) NOT NULL,
	[point] [int] NOT NULL,
	[Count] [int] NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
 CONSTRAINT [PK_ReportDealLogDay] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportSettings]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportSettings](
	[ReportSettingId] [int] IDENTITY(1,1) NOT NULL,
	[ReportType] [nvarchar](500) NULL,
	[Value] [nvarchar](1000) NOT NULL,
	[isEnabled] [bit] NULL,
 CONSTRAINT [PK_ReportSettings] PRIMARY KEY CLUSTERED 
(
	[ReportSettingId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportShopDeals]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportShopDeals](
	[ShopDealsReportId] [int] IDENTITY(1,1) NOT NULL,
	[ShopId] [int] NOT NULL,
	[ShopName] [nvarchar](50) NOT NULL,
	[PayAmount] [decimal](18, 2) NOT NULL,
	[PayCount] [int] NOT NULL,
	[DonePrepayAmount] [decimal](18, 2) NOT NULL,
	[DonePrepayCount] [int] NOT NULL,
	[CancelAmount] [decimal](18, 2) NOT NULL,
	[CancelCount] [int] NOT NULL,
	[CancelDonePrepayAmount] [decimal](18, 2) NOT NULL,
	[CancelDonePrepayCount] [int] NOT NULL,
	[UnPayCount] [int] NOT NULL,
	[SubmitDate] [datetime] NOT NULL,
	[ShopDealLogDoneAmount] [decimal](18, 2) NULL,
	[ShopDealLogChargeAmount] [decimal](18, 2) NULL,
 CONSTRAINT [PK_ShopDealsReport] PRIMARY KEY CLUSTERED 
(
	[ShopDealsReportId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportShopMonth]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportShopMonth](
	[ShopMonthtId] [int] IDENTITY(1,1) NOT NULL,
	[ShopId] [int] NOT NULL,
	[ShopName] [nvarchar](50) NOT NULL,
	[DealAmount] [decimal](18, 2) NOT NULL,
	[CancelAmount] [decimal](18, 2) NOT NULL,
	[Month] [nvarchar](50) NOT NULL,
 CONSTRAINT [PK_ShopMonthReport] PRIMARY KEY CLUSTERED 
(
	[ShopMonthtId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportSystemDealLogByUser]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportSystemDealLogByUser](
	[SystemDealLogByUserId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitDate] [nvarchar](20) NOT NULL,
	[UserId] [int] NOT NULL,
	[SumAmount] [decimal](18, 2) NOT NULL,
	[AvgAmount] [decimal](18, 2) NOT NULL,
	[Count] [int] NOT NULL,
	[DealType] [int] NOT NULL,
 CONSTRAINT [PK_ReportSystemDealLogByUser] PRIMARY KEY CLUSTERED 
(
	[SystemDealLogByUserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[ReportSystemDealLogDay]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ReportSystemDealLogDay](
	[SystemDealLogDayId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitDate] [nvarchar](20) NOT NULL,
	[SumAmount] [decimal](18, 2) NOT NULL,
	[AvgAmount] [decimal](18, 2) NOT NULL,
	[Count] [int] NOT NULL,
	[DealType] [int] NOT NULL,
 CONSTRAINT [PK_ReportSystemDealLogDay] PRIMARY KEY CLUSTERED 
(
	[SystemDealLogDayId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Roles]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Roles](
	[RoleId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DisplayName] [nvarchar](100) NULL,
	[Description] [nvarchar](2000) NULL,
	[BuildIn] [bit] NOT NULL,
	[Permissions] [nvarchar](4000) NULL,
	[IsSuper] [bit] NOT NULL,
	[State] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[RollbackShopDealLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[RollbackShopDealLogs](
	[RollbackShopDealLogId] [int] IDENTITY(1,1) NOT NULL,
	[ShopDealLogId] [int] NOT NULL,
	[ShopId] [int] NOT NULL,
	[State] [int] NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
 CONSTRAINT [PK_RollbackShopDealLogs] PRIMARY KEY CLUSTERED 
(
	[RollbackShopDealLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SetWeChats]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
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

GO
/****** Object:  Table [dbo].[ShopDealLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[ShopDealLogs](
	[ShopDealLogId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[SourceShopId] [int] NOT NULL,
	[SourceShopName] [nvarchar](100) NULL,
	[SourcePosId] [int] NOT NULL,
	[SourcePosName] [nvarchar](100) NULL,
	[State] [int] NOT NULL,
	[AccountId] [int] NULL,
	[AccountName] [nvarchar](100) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[SerialNo] [nvarchar](100) NULL,
	[DealType] [int] NOT NULL,
	[SourceShopDisplayName] [nvarchar](100) NULL,
	[ShopAmount] [decimal](18, 2) NOT NULL,
	[Addin] [int] NOT NULL,
	[shopId] [int] NULL,
	[shopName] [nvarchar](50) NULL,
	[shopDisplayName] [nvarchar](50) NULL,
	[LiquidateDealLogId] [int] NOT NULL,
	[ShopRechargingAmount] [decimal](18, 2) NULL,
	[Code] [nvarchar](50) NULL,
PRIMARY KEY CLUSTERED 
(
	[ShopDealLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Shops]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Shops](
	[ShopId] [int] IDENTITY(1,1) NOT NULL,
	[State] [int] NOT NULL,
	[ShopDealLogChargeRate] [decimal](18, 2) NULL,
	[EnableShopMakeCard] [bit] NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DisplayName] [nvarchar](100) NULL,
	[Address] [nvarchar](1000) NULL,
	[PhoneNumber] [nvarchar](100) NULL,
	[PhoneNumber2] [nvarchar](100) NULL,
	[Bank] [nvarchar](100) NULL,
	[BankAccount] [nvarchar](100) NULL,
	[ShopDealLogAmount] [nvarchar](100) NULL,
	[ShopType] [int] NULL,
	[buildin] [int] NULL,
	[mobile] [nvarchar](50) NULL,
	[mobile2] [nvarchar](50) NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[RecordVersion] [int] NOT NULL,
	[RechargingAmount] [decimal](18, 2) NULL,
	[BankAccountName] [nvarchar](30) NULL,
	[DealWayId] [int] NULL,
	[Description] [nvarchar](30) NULL,
	[BankUserName] [nvarchar](50) NULL,
	[bankPoint] [nvarchar](100) NULL,
	[RechargeAmount] [decimal](18, 2) NULL,
	[shopDiscount] [decimal](18, 3) NULL,
PRIMARY KEY CLUSTERED 
(
	[ShopId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SiteAccounts]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SiteAccounts](
	[SiteAccountId] [int] IDENTITY(1,1) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[RecordVersion] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteAccountId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sites]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sites](
	[SiteId] [int] IDENTITY(1,1) NOT NULL,
	[Name] [nvarchar](100) NULL,
	[DisplayName] [nvarchar](100) NULL,
	[Description] [nvarchar](100) NULL,
	[FavIconUrl] [nvarchar](100) NULL,
	[Version] [decimal](18, 4) NULL,
	[ServiceRetryCountDefault] [tinyint] NOT NULL,
	[RouteUrlPrefix] [nvarchar](200) NULL,
	[CommentingDisabled] [bit] NOT NULL,
	[MixCode] [nvarchar](200) NULL,
	[State] [int] NOT NULL,
	[SendMessageForBirthDay] [bit] NOT NULL,
	[IsIKeySignIn] [bit] NOT NULL,
	[ShopDealLogChargeRate] [decimal](18, 2) NOT NULL,
	[SaleCardFee] [decimal](18, 2) NULL,
	[ChangeCardFee] [decimal](18, 2) NULL,
	[IncludeOpenSearch] [bit] NOT NULL,
	[AccountNameLength] [int] NOT NULL,
	[MessageTemplateOfDeal] [nvarchar](2000) NULL,
	[MessageTemplateOfBirthDate] [nvarchar](256) NULL,
	[PosType] [nvarchar](256) NULL,
	[AccountToken] [nvarchar](256) NULL,
	[MessageTemplateOfIdentity] [nvarchar](256) NULL,
	[MessageTemplateOfRecharge] [nvarchar](512) NULL,
	[MessageTemplateOfOpenReceipt] [nvarchar](512) NULL,
	[AuthType] [nvarchar](100) NULL,
	[printerType] [nvarchar](100) NULL,
	[MessageTemplateOfUnIdentity] [nvarchar](1000) NULL,
	[TicketTemplateOfRecharge] [nvarchar](4000) NULL,
	[PasswordType] [nvarchar](1000) NULL,
	[TicketTemplateOfClose] [nvarchar](1000) NULL,
	[TicketTemplateOfDeal] [nvarchar](1000) NULL,
	[TicketTemplateOfCancelDeal] [nvarchar](1000) NULL,
	[TimeOutOfCancelSystemDeal] [int] NULL,
	[TicketTemplateOfTransfer] [nvarchar](1000) NULL,
	[TicketTemplateOfSuspendAccount] [nvarchar](1000) NULL,
	[TicketTemplateOfResumeAccount] [nvarchar](1000) NULL,
	[TicketTemplateOfChangeAccountName] [nvarchar](1000) NULL,
	[TicketTemplateOfOpen] [nvarchar](1000) NULL,
	[TicketTemplateOfRenewAccount] [nvarchar](1000) NULL,
	[MessageTemplateOfPrePay] [nvarchar](2000) NULL,
	[MessageTemplateOfDonePrePay] [nvarchar](2000) NULL,
	[MessageTemplateOfTicket] [nvarchar](2000) NULL,
	[MessageTemplateOfAccountResume] [nvarchar](2000) NULL,
	[MessageTemplateOfAccountSuspend] [nvarchar](2000) NULL,
	[MessageTemplateOfAccountTransfer] [nvarchar](2000) NULL,
	[MessageTemplateOfAccountChangeName] [nvarchar](2000) NULL,
	[MessageTemplateOfAccountRenew] [nvarchar](2000) NULL,
	[Banks] [nvarchar](1000) NULL,
	[MessageTemplateOfShopDeal] [nvarchar](1000) NULL,
	[CopyRight] [nvarchar](4000) NULL,
	[MessageTemplateOfDealCode] [nvarchar](1000) NULL,
	[IsRechargingApprove] [bit] NULL,
	[IsLimiteAmountApprove] [bit] NULL,
	[DistributorDealLogChargeRate] [decimal](18, 2) NULL,
	[HowToDeals] [nvarchar](1000) NULL,
	[SmsAccount] [nvarchar](50) NULL,
	[SmsPwd] [nvarchar](50) NULL,
	[RetryCount] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SiteId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Sms]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Sms](
	[SmsId] [int] IDENTITY(1,1) NOT NULL,
	[Message] [nvarchar](4000) NULL,
	[RetryCount] [int] NOT NULL,
	[Mobile] [nvarchar](20) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[ReservateTime] [datetime] NULL,
	[ExpiredTime] [datetime] NULL,
	[State] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[SmsId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[SystemDealLogs]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[SystemDealLogs](
	[SystemDealLogId] [int] IDENTITY(1,1) NOT NULL,
	[DealType] [int] NOT NULL,
	[Amount] [decimal](18, 2) NOT NULL,
	[Addin] [nvarchar](100) NULL,
	[SubmitTime] [datetime] NOT NULL,
	[UserId] [int] NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[HasReceipt] [bit] NULL,
	[HowToDeal] [nvarchar](100) NULL,
	[state] [int] NULL,
	[SerialNo] [nvarchar](1000) NULL,
	[DealWayId] [int] NULL,
	[siteAmount] [decimal](18, 2) NULL,
	[ShopName] [nvarchar](50) NULL,
	[Operator] [nvarchar](50) NULL,
	[accountName] [nvarchar](50) NULL,
	[DistributorName] [int] NULL,
	[DistributorId] [int] NULL,
PRIMARY KEY CLUSTERED 
(
	[SystemDealLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tasks]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Tasks](
	[TaskId] [int] IDENTITY(1,1) NOT NULL,
	[CommandTypeName] [nvarchar](1000) NOT NULL,
	[CommandParameter] [nvarchar](max) NOT NULL,
	[SubmitTime] [datetime] NOT NULL,
	[ExecuteTime] [datetime] NULL,
	[Error] [nvarchar](max) NULL,
	[EditorId] [int] NULL,
	[CreatorId] [int] NULL,
	[AccountId] [int] NULL,
	[State] [int] NOT NULL,
	[Amount] [decimal](18, 2) NULL,
	[RecordVersion] [int] NULL,
 CONSTRAINT [PK_Tasks] PRIMARY KEY CLUSTERED 
(
	[TaskId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY] TEXTIMAGE_ON [PRIMARY]

GO
/****** Object:  Table [dbo].[TemporaryTokenKeys]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TemporaryTokenKeys](
	[TemporaryTokenKeyId] [int] IDENTITY(1,1) NOT NULL,
	[UserName] [nvarchar](100) NULL,
	[Token] [nvarchar](100) NULL,
	[ExpiredDate] [datetime] NOT NULL,
	[TokenKeyType] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[TemporaryTokenKeyId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Tickets]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
SET ANSI_PADDING ON
GO
CREATE TABLE [dbo].[Tickets](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NULL,
	[orderNo] [varchar](100) NULL,
	[AdmissionTicketId] [int] NULL,
	[Code] [nvarchar](200) NULL,
	[ExpiredDate] [datetime] NULL,
	[Price] [decimal](18, 2) NULL,
	[adultNum] [int] NULL,
	[childNum] [int] NULL,
	[useScope] [nvarchar](200) NULL,
	[State] [int] NULL,
	[BuyTime] [datetime] NULL,
	[userTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
SET ANSI_PADDING OFF
GO
/****** Object:  Table [dbo].[TypeNames]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[TypeNames](
	[Id] [int] IDENTITY(1,1) NOT NULL,
	[TypeId] [int] NOT NULL,
	[Text] [nvarchar](50) NOT NULL,
	[Category] [nvarchar](50) NOT NULL,
	[OrderNum] [int] NOT NULL,
 CONSTRAINT [PK_States] PRIMARY KEY CLUSTERED 
(
	[Id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UseCouponslog]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UseCouponslog](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[userId] [int] NULL,
	[couponsId] [int] NULL,
	[orderNo] [nvarchar](100) NULL,
	[amount] [decimal](18, 2) NULL,
	[discount] [decimal](18, 2) NULL,
	[useTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserCoupons]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserCoupons](
	[id] [int] IDENTITY(1,1) NOT NULL,
	[couponsId] [int] NULL,
	[userId] [int] NULL,
	[state] [int] NULL,
	[receiveTime] [datetime] NULL,
	[useTime] [datetime] NULL,
PRIMARY KEY CLUSTERED 
(
	[id] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[UserRoles]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[UserRoles](
	[User_UserId] [int] NOT NULL,
	[Role_RoleId] [int] NOT NULL,
PRIMARY KEY CLUSTERED 
(
	[User_UserId] ASC,
	[Role_RoleId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  Table [dbo].[Users]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE TABLE [dbo].[Users](
	[UserId] [int] IDENTITY(1,1) NOT NULL,
	[LastSignInTime] [datetime] NULL,
	[Email] [nvarchar](40) NULL,
	[Password] [nvarchar](100) NULL,
	[PasswordSalt] [nvarchar](100) NULL,
	[State] [int] NOT NULL,
	[BirthDate] [datetime] NULL,
	[BuildIn] [bit] NOT NULL,
	[Mobile] [nvarchar](100) NULL,
	[LoginToken] [nvarchar](100) NULL,
	[LoginInToken] [bit] NOT NULL,
	[Name] [nvarchar](50) NULL,
	[DisplayName] [nvarchar](50) NULL,
	[ShopRole] [int] NULL,
	[ShopId] [int] NULL,
	[Discriminator] [nvarchar](128) NOT NULL,
	[photo] [nvarchar](50) NULL,
	[SignOnTime] [datetime] NULL,
	[Gender] [int] NULL,
	[IdentityCard] [nvarchar](50) NULL,
	[Address] [nvarchar](50) NULL,
	[Mobile2] [nvarchar](256) NULL,
	[PhoneNumber] [nvarchar](256) NULL,
	[PhoneNumber2] [nvarchar](256) NULL,
	[isMobileAvailable] [bit] NULL,
	[isSale] [bit] NULL,
	[DistributorId] [int] NULL,
	[Age] [int] NULL,
	[Mark] [nvarchar](256) NULL,
	[babyName] [nvarchar](100) NULL,
	[babySex] [int] NULL,
	[babyBirthDate] [datetime] NULL,
	[openId] [nvarchar](200) NULL,
PRIMARY KEY CLUSTERED 
(
	[UserId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, IGNORE_DUP_KEY = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
) ON [PRIMARY]

GO
/****** Object:  View [dbo].[V_DistributorRate]    Script Date: 2018/10/25 20:06:28 ******/
SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO
CREATE VIEW [dbo].[V_DistributorRate]
AS
SELECT     dbo.DealLogs.AccountId, dbo.DealLogs.Amount, dbo.DistributorAccountLevelPolicyRates.Rate, dbo.DistributorAccountLevelPolicyRates.DistributorId, 
                      dbo.DealLogs.SubmitTime, dbo.DistributorAccountLevelPolicyRates.AccountLevelPolicyId, dbo.Distributors.ParentId
FROM         dbo.DistributorAccountLevelPolicyRates INNER JOIN
                      dbo.Distributors ON dbo.DistributorAccountLevelPolicyRates.DistributorId = dbo.Distributors.DistributorId LEFT OUTER JOIN
                      dbo.Accounts ON dbo.DistributorAccountLevelPolicyRates.DistributorId = dbo.Accounts.DistributorId RIGHT OUTER JOIN
                      dbo.DealLogs ON dbo.Accounts.AccountId = dbo.DealLogs.AccountId
WHERE     (dbo.DealLogs.DealType IN (1, 4, 8))


GO
SET IDENTITY_INSERT [dbo].[Accounts] ON 

GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (1, 2, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649138 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 5, NULL, N'0000000000000001', N'Rx6a4aPGvc6hRkkLhFe0BUnlXcI4/E+AbxTvDpkQgqw=', N'506364', N'35891321', CAST(0x0000A95001649169 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (2, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649C43 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000002', N'n8BLW6rqHj2Q/wgJ49HTluLBsta43BpVMP+Lcse4wes=', N'454577', N'81604869', CAST(0x0000A95001649C43 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (3, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649C70 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000003', N'42J+K8kY09rAQq7OavUU4QJlLLmIkf7zz+cy3zN2/SQ=', N'645688', N'36697569', CAST(0x0000A95001649C70 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (4, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649C9A AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000004', N'n1xEGNXF7Ww0nTuNiWi4D3lT/HeVgpCmPz7aWCWk0qk=', N'773952', N'79246068', CAST(0x0000A95001649C9A AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (5, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649CC6 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000005', N'bsRy42+UUoXGS4rpU0+91vWGLwCiXXMwucQUpuHKYkQ=', N'688619', N'54003676', CAST(0x0000A95001649CC6 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (6, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649CEF AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000006', N'5ax3kd3wsAqeayCkI7XnStbhVAkA2nKNtsa43OrXgBI=', N'030758', N'52819276', CAST(0x0000A95001649CEF AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (7, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649D1A AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000007', N'P61TcUCrgNun9W3F3kWHtnrgUYb7KSv+omylepGw5Qw=', N'593921', N'50619821', CAST(0x0000A95001649D1A AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (8, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649D45 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000008', N'x+Ik/rtseIRQWpKEvVv3iNF86b5/8T7RpMwayeEg9Yg=', N'447286', N'97972434', CAST(0x0000A95001649D45 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (9, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649D76 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000009', N'kLmFWG4+ySWgrt0DUVocLaOo90jBgvlQH6SCZj1LHRA=', N'851584', N'01213889', CAST(0x0000A95001649D76 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (10, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A9AB00000000 AS DateTime), CAST(0x0000A95001649DA0 AS DateTime), 1, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 3, NULL, N'0000000000000010', N'JuPeHFgqCWfpEtuUgsYTXD9KCyBsMIY1YC3FLAIKsEM=', N'025261', N'84732622', CAST(0x0000A95001649DA0 AS DateTime), 0, 0, N'批量开卡', CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (11, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A95001657EA0 AS DateTime), CAST(0x0000A95001657EA0 AS DateTime), 11, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'0000000000000011', N'39DctFb1KyKuDf+q2R86OzEIW3U6RiaoNAEN9qf8kJs=', N'286886', N'96733715', NULL, 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (12, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A95001657EA0 AS DateTime), CAST(0x0000A95001657EA0 AS DateTime), 11, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'0000000000000012', N'oAaaGbZApQ2M3RTX0cWLEp0LYtWsWvTVu/dH7FCr8BM=', N'970108', N'76303838', NULL, 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (13, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A95001657EA0 AS DateTime), CAST(0x0000A95001657EA0 AS DateTime), 11, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'0000000000000013', N'fYSlWRDBvstZMRHwqOxBWnMMjsu/UMsdzocTdDX5DWk=', N'203285', N'23397151', NULL, 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (14, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 3, CAST(0x0000A95001657EA0 AS DateTime), CAST(0x0000A95001657EA0 AS DateTime), 11, 5, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'0000000000000014', N'PqgljRiWeGRW69JXEydAFGVRin98of0jxnvne2Gv5SA=', N'467120', N'04938987', NULL, 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), -10001, NULL, 5, 0, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (16, 4, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, CAST(0x0000AAF10146A860 AS DateTime), CAST(0x0000A9840146A860 AS DateTime), 1, 2, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'6000000000000001', NULL, NULL, NULL, CAST(0x0000A9840146A860 AS DateTime), 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, NULL, 50, 0, N'', CAST(5000.00 AS Decimal(18, 2)), 50, CAST(100.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (17, 4, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, CAST(0x0000AAF10146B805 AS DateTime), CAST(0x0000A9840146B805 AS DateTime), 1, 1, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'6000000000000002', NULL, NULL, NULL, CAST(0x0000A9840146B805 AS DateTime), 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, NULL, 20, 0, N'', CAST(3000.00 AS Decimal(18, 2)), 20, CAST(150.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (18, 4, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, CAST(0x0000AAF1014877DB AS DateTime), CAST(0x0000A984014877DB AS DateTime), 1, 3, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'6000000000000003', NULL, NULL, NULL, CAST(0x0000A984014877DB AS DateTime), 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, NULL, 0, 0, N'', CAST(6000.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[Accounts] ([AccountId], [OwnerId], [Amount], [FreezeAmount], [ExpiredMonths], [ExpiredDate], [LastDealTime], [State], [AccountTypeId], [ShopId], [PayAmount], [DepositAmount], [AccountToken], [Point], [TotalPoint], [AccountLevel], [RecordVersion], [AccountLevelName], [Name], [Password], [InitPassword], [PasswordSalt], [OpenTime], [IsMessageOfDeal], [SalerId], [Remark1], [ChargingAmount], [limiteAmount], [DistributorId], [IsRecharging], [frequency], [frequencyUsed], [useScope], [SaleAmount], [TotalTimes], [SinglePrice]) VALUES (19, 4, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, CAST(0x0000AAF101487FA0 AS DateTime), CAST(0x0000A98401487FA0 AS DateTime), 1, 4, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), N'11111111', 0, 0, 0, 0, NULL, N'6000000000000004', NULL, NULL, NULL, CAST(0x0000A98401487FA0 AS DateTime), 0, 0, NULL, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 0, NULL, 0, 0, N'', CAST(8800.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[Accounts] OFF
GO
SET IDENTITY_INSERT [dbo].[AccountTypes] ON 

GO
INSERT [dbo].[AccountTypes] ([AccountTypeId], [DisplayName], [IsRecharging], [Amount], [Point], [ExpiredMonths], [RenewMonths], [IsRenew], [DepositAmount], [State], [IsMessageOfDeal], [IsPointable], [IsSmsDeal], [IsSmsAccountBirthday], [IsSmsRecharge], [IsSmsTransfer], [IsSmsSuspend], [IsSmsResume], [IsSmsRenew], [IsSmsClose], [IsSmsMobile], [IsSmscancelMobile], [IsSmsCode], [IsSmsChangeName], [frequency], [describe], [numberOfPeople]) VALUES (1, N'白银气球卡', 1, CAST(3000.00 AS Decimal(18, 2)), 0, 12, 1000, 1, CAST(0.00 AS Decimal(18, 2)), 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, 1, NULL, NULL, 1, 1, 20, N'一年有效，不限周末平时，每次最多划3个小朋友', 3)
GO
INSERT [dbo].[AccountTypes] ([AccountTypeId], [DisplayName], [IsRecharging], [Amount], [Point], [ExpiredMonths], [RenewMonths], [IsRenew], [DepositAmount], [State], [IsMessageOfDeal], [IsPointable], [IsSmsDeal], [IsSmsAccountBirthday], [IsSmsRecharge], [IsSmsTransfer], [IsSmsSuspend], [IsSmsResume], [IsSmsRenew], [IsSmsClose], [IsSmsMobile], [IsSmscancelMobile], [IsSmsCode], [IsSmsChangeName], [frequency], [describe], [numberOfPeople]) VALUES (2, N'黄金气球卡', 0, CAST(5000.00 AS Decimal(18, 2)), 0, 12, 0, 0, CAST(0.00 AS Decimal(18, 2)), 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, 0, 0, 50, N'一年有效，不限周末平时，每次最多划3个小朋友', 3)
GO
INSERT [dbo].[AccountTypes] ([AccountTypeId], [DisplayName], [IsRecharging], [Amount], [Point], [ExpiredMonths], [RenewMonths], [IsRenew], [DepositAmount], [State], [IsMessageOfDeal], [IsPointable], [IsSmsDeal], [IsSmsAccountBirthday], [IsSmsRecharge], [IsSmsTransfer], [IsSmsSuspend], [IsSmsResume], [IsSmsRenew], [IsSmsClose], [IsSmsMobile], [IsSmscancelMobile], [IsSmsCode], [IsSmsChangeName], [frequency], [describe], [numberOfPeople]) VALUES (3, N'铂金气球卡', 0, CAST(6000.00 AS Decimal(18, 2)), 0, 12, 0, 0, CAST(0.00 AS Decimal(18, 2)), 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, 0, 0, 0, N'一年有效，只限周一到周五，每次划1个小朋友', 1)
GO
INSERT [dbo].[AccountTypes] ([AccountTypeId], [DisplayName], [IsRecharging], [Amount], [Point], [ExpiredMonths], [RenewMonths], [IsRenew], [DepositAmount], [State], [IsMessageOfDeal], [IsPointable], [IsSmsDeal], [IsSmsAccountBirthday], [IsSmsRecharge], [IsSmsTransfer], [IsSmsSuspend], [IsSmsResume], [IsSmsRenew], [IsSmsClose], [IsSmsMobile], [IsSmscancelMobile], [IsSmsCode], [IsSmsChangeName], [frequency], [describe], [numberOfPeople]) VALUES (4, N'黑金气球卡', 0, CAST(8800.00 AS Decimal(18, 2)), 0, 12, 0, 0, CAST(0.00 AS Decimal(18, 2)), 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, 0, 0, 0, N'一年有效不限周末平时，一次划1个小朋友', 1)
GO
INSERT [dbo].[AccountTypes] ([AccountTypeId], [DisplayName], [IsRecharging], [Amount], [Point], [ExpiredMonths], [RenewMonths], [IsRenew], [DepositAmount], [State], [IsMessageOfDeal], [IsPointable], [IsSmsDeal], [IsSmsAccountBirthday], [IsSmsRecharge], [IsSmsTransfer], [IsSmsSuspend], [IsSmsResume], [IsSmsRenew], [IsSmsClose], [IsSmsMobile], [IsSmscancelMobile], [IsSmsCode], [IsSmsChangeName], [frequency], [describe], [numberOfPeople]) VALUES (5, N'礼品卡', 0, CAST(0.00 AS Decimal(18, 2)), 0, 3, 0, 0, CAST(0.00 AS Decimal(18, 2)), 1, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, NULL, NULL, 0, 0, 5, N'礼品卡赠送使用', 1)
GO
SET IDENTITY_INSERT [dbo].[AccountTypes] OFF
GO
SET IDENTITY_INSERT [dbo].[AdmissionTicket] ON 

GO
INSERT [dbo].[AdmissionTicket] ([id], [name], [adultNum], [childNum], [addAdultAmount], [amount], [weekendAmount], [state], [introduce], [details], [crateTime]) VALUES (1, N'儿童套餐票', 1, 2, CAST(0.00 AS Decimal(18, 2)), CAST(228.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), 1, N'内含一大一小套票当天有效', NULL, CAST(0x0000A956009E9EF8 AS DateTime))
GO
INSERT [dbo].[AdmissionTicket] ([id], [name], [adultNum], [childNum], [addAdultAmount], [amount], [weekendAmount], [state], [introduce], [details], [crateTime]) VALUES (3, N'家庭乐园套票', 1, 1, CAST(0.00 AS Decimal(18, 2)), CAST(216.00 AS Decimal(18, 2)), CAST(250.00 AS Decimal(18, 2)), 1, N'内含1大1小套票，成人两人及以上请另购成人票。', NULL, CAST(0x0000A97C015F2ADB AS DateTime))
GO
INSERT [dbo].[AdmissionTicket] ([id], [name], [adultNum], [childNum], [addAdultAmount], [amount], [weekendAmount], [state], [introduce], [details], [crateTime]) VALUES (4, N'下午茶套票', 1, 1, CAST(0.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), CAST(200.00 AS Decimal(18, 2)), 1, N'内含1大1小套票，成人两人及以上请另购成人票。', NULL, CAST(0x0000A97C015FDCE2 AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[AdmissionTicket] OFF
GO
SET IDENTITY_INSERT [dbo].[Coupons] ON 

GO
INSERT [dbo].[Coupons] ([id], [code], [couponsType], [name], [useScope], [discount], [deductibleAmount], [fullAmount], [reduceAmount], [quantity], [validity], [leadersOfNum], [state], [createOp], [createTime]) VALUES (1, N'201809091513245067', 1, N'折扣卷', N'000000000000001', CAST(0.80 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 10, CAST(0x0000A96E00000000 AS DateTime), 1, 1, N'admin', CAST(0x0000A9560105EF08 AS DateTime))
GO
INSERT [dbo].[Coupons] ([id], [code], [couponsType], [name], [useScope], [discount], [deductibleAmount], [fullAmount], [reduceAmount], [quantity], [validity], [leadersOfNum], [state], [createOp], [createTime]) VALUES (3, N'201810141850347755', 2, N'抵扣卷', N'', CAST(0.00 AS Decimal(18, 2)), CAST(50.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)), 50, CAST(0x0000A97100000000 AS DateTime), 2, 1, N'admin', CAST(0x0000A979013685E1 AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[Coupons] OFF
GO
SET IDENTITY_INSERT [dbo].[DealLogs] ON 

GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (1, CAST(0x0000A95001648E4C AS DateTime), 0, NULL, 0, NULL, 1, 1, N'0000000000000001', CAST(0.00 AS Decimal(18, 2)), N'WN1318045548877276800001', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (2, CAST(0x0000A95001649C43 AS DateTime), 0, NULL, 0, NULL, 1, 2, N'0000000000000002', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550262885940002', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (3, CAST(0x0000A95001649C70 AS DateTime), 0, NULL, 0, NULL, 1, 3, N'0000000000000003', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550277377690003', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (4, CAST(0x0000A95001649C9A AS DateTime), 0, NULL, 0, NULL, 1, 4, N'0000000000000004', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550292069300004', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (5, CAST(0x0000A95001649CC6 AS DateTime), 0, NULL, 0, NULL, 1, 5, N'0000000000000005', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550306061300005', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (6, CAST(0x0000A95001649CEF AS DateTime), 0, NULL, 0, NULL, 1, 6, N'0000000000000006', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550320553110006', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (7, CAST(0x0000A95001649D1A AS DateTime), 0, NULL, 0, NULL, 1, 7, N'0000000000000007', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550334645010007', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (8, CAST(0x0000A95001649D45 AS DateTime), 0, NULL, 0, NULL, 1, 8, N'0000000000000008', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550348836900008', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (9, CAST(0x0000A95001649D76 AS DateTime), 0, NULL, 0, NULL, 1, 9, N'0000000000000009', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550364627910009', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
INSERT [dbo].[DealLogs] ([DealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [AccountAmount], [Addin], [Point], [IsHidden], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [YAmount], [DiscountRate]) VALUES (10, CAST(0x0000A95001649DA0 AS DateTime), 0, NULL, 0, NULL, 1, 10, N'0000000000000010', CAST(0.00 AS Decimal(18, 2)), N'WN1318045550379319490010', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 0, 0, 0, NULL, NULL, 0, CAST(0.00 AS Decimal(18, 2)), CAST(0.00 AS Decimal(18, 2)))
GO
SET IDENTITY_INSERT [dbo].[DealLogs] OFF
GO
SET IDENTITY_INSERT [dbo].[DealWays] ON 

GO
INSERT [dbo].[DealWays] ([DealWayId], [DisplayName], [ApplyTo], [State], [isCash]) VALUES (10, N'现金支付', 15, 1, 0)
GO
INSERT [dbo].[DealWays] ([DealWayId], [DisplayName], [ApplyTo], [State], [isCash]) VALUES (11, N'银行卡支付', 15, 1, 0)
GO
SET IDENTITY_INSERT [dbo].[DealWays] OFF
GO
SET IDENTITY_INSERT [dbo].[fz_Articles] ON 

GO
INSERT [dbo].[fz_Articles] ([articleId], [title], [imageUrl], [describe], [submitTime]) VALUES (1, N'活动预约', NULL, N'<p>
	<img src="/editorImages/20180909231722_1257.png" alt="" />
</p>
<p>
	123123123
</p>', CAST(0x0000A956017FD415 AS DateTime))
GO
INSERT [dbo].[fz_Articles] ([articleId], [title], [imageUrl], [describe], [submitTime]) VALUES (2, N'亲子活动列表', NULL, N'', NULL)
GO
INSERT [dbo].[fz_Articles] ([articleId], [title], [imageUrl], [describe], [submitTime]) VALUES (3, N'精彩回顾', NULL, N'', NULL)
GO
INSERT [dbo].[fz_Articles] ([articleId], [title], [imageUrl], [describe], [submitTime]) VALUES (4, N'精选活动', NULL, N'', NULL)
GO
INSERT [dbo].[fz_Articles] ([articleId], [title], [imageUrl], [describe], [submitTime]) VALUES (5, N'联系我们', NULL, N'', NULL)
GO
SET IDENTITY_INSERT [dbo].[fz_Articles] OFF
GO
SET IDENTITY_INSERT [dbo].[fz_OrderDetial] ON 

GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1002, N'2018102519490745254', CAST(6000.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A98401469A4C AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1003, N'2018102519491483124', CAST(6000.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A9840146A2F1 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1004, N'2018102519491944934', CAST(5000.00 AS Decimal(18, 2)), 2, 1, N'', CAST(0x0000A9840146A85B AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1005, N'2018102519492757774', CAST(6000.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A9840146B1E1 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1006, N'2018102519493051264', CAST(8800.00 AS Decimal(18, 2)), 4, 1, N'', CAST(0x0000A9840146B552 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1007, N'2018102519493281384', CAST(3000.00 AS Decimal(18, 2)), 1, 1, N'', CAST(0x0000A9840146B804 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1008, N'2018102519502490434', CAST(6000.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A9840146F50F AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1009, N'2018102519513090294', CAST(6000.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A98401474AE9 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1010, N'2018102519555490044', CAST(6000.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A984014877C6 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1011, N'2018102519560158664', CAST(8800.00 AS Decimal(18, 2)), 4, 1, N'', CAST(0x0000A98401487F9C AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1012, N'2018102519570140874', CAST(216.00 AS Decimal(18, 2)), 3, 1, N'', CAST(0x0000A9840148C5B7 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1013, N'2018102519570140874', CAST(200.00 AS Decimal(18, 2)), 4, 1, N'', CAST(0x0000A9840148C5B8 AS DateTime))
GO
INSERT [dbo].[fz_OrderDetial] ([id], [orderNo], [amount], [sourceId], [num], [cardNo], [subTime]) VALUES (1014, N'2018102519575143924', CAST(228.00 AS Decimal(18, 2)), 1, 1, N'', CAST(0x0000A98401490058 AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[fz_OrderDetial] OFF
GO
SET IDENTITY_INSERT [dbo].[fz_Orders] ON 

GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1003, 4, N'2018102519490745254', CAST(6000.00 AS Decimal(18, 2)), 1, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(6000.00 AS Decimal(18, 2)), NULL, CAST(0x0000A98401469A4C AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1004, 4, N'2018102519491483124', CAST(6000.00 AS Decimal(18, 2)), 1, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(6000.00 AS Decimal(18, 2)), NULL, CAST(0x0000A9840146A2F1 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1005, 4, N'2018102519491944934', CAST(5000.00 AS Decimal(18, 2)), 2, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(5000.00 AS Decimal(18, 2)), CAST(0x0000A9840146A85F AS DateTime), CAST(0x0000A9840146A85B AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1006, 4, N'2018102519492757774', CAST(6000.00 AS Decimal(18, 2)), 1, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(6000.00 AS Decimal(18, 2)), NULL, CAST(0x0000A9840146B1E1 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1007, 4, N'2018102519493051264', CAST(8800.00 AS Decimal(18, 2)), 1, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(8800.00 AS Decimal(18, 2)), NULL, CAST(0x0000A9840146B552 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1008, 4, N'2018102519493281384', CAST(3000.00 AS Decimal(18, 2)), 2, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(3000.00 AS Decimal(18, 2)), CAST(0x0000A9840146B804 AS DateTime), CAST(0x0000A9840146B804 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1009, 4, N'2018102519502490434', CAST(6000.00 AS Decimal(18, 2)), 1, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(6000.00 AS Decimal(18, 2)), NULL, CAST(0x0000A9840146F50F AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1010, 4, N'2018102519513090294', CAST(6000.00 AS Decimal(18, 2)), 1, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(6000.00 AS Decimal(18, 2)), NULL, CAST(0x0000A984014747D6 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1011, 4, N'2018102519555490044', CAST(6000.00 AS Decimal(18, 2)), 2, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(6000.00 AS Decimal(18, 2)), CAST(0x0000A984014877CE AS DateTime), CAST(0x0000A984014877C6 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1012, 4, N'2018102519560158664', CAST(8800.00 AS Decimal(18, 2)), 2, 2, CAST(0.00 AS Decimal(18, 2)), N'', CAST(8800.00 AS Decimal(18, 2)), CAST(0x0000A98401487F9D AS DateTime), CAST(0x0000A98401487F9C AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1013, 4, N'2018102519570140874', CAST(416.00 AS Decimal(18, 2)), 2, 1, CAST(0.00 AS Decimal(18, 2)), N'', CAST(416.00 AS Decimal(18, 2)), CAST(0x0000A9840148C5B8 AS DateTime), CAST(0x0000A9840148C5B8 AS DateTime))
GO
INSERT [dbo].[fz_Orders] ([id], [userId], [orderNo], [amount], [orderState], [type], [deductible], [useScope], [payAmount], [payTime], [subTime]) VALUES (1014, 4, N'2018102519575143924', CAST(228.00 AS Decimal(18, 2)), 2, 1, CAST(0.00 AS Decimal(18, 2)), N'', CAST(228.00 AS Decimal(18, 2)), CAST(0x0000A98401490059 AS DateTime), CAST(0x0000A98401490058 AS DateTime))
GO
SET IDENTITY_INSERT [dbo].[fz_Orders] OFF
GO
SET IDENTITY_INSERT [dbo].[Logs] ON 

GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5333, CAST(0x0000A4AD00BDACEE AS DateTime), 1, N'admin', N'积分规则删除', N'删除积分规则 积分规则02', 126, 5, N'WN1307786223524475420003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5334, CAST(0x0000A4AD00BDC7E5 AS DateTime), 1, N'admin', N'删除礼品', N'删除礼品 玩具', 176, 1, N'WN1307786225825507030004')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5335, CAST(0x0000A94C0169E9D3 AS DateTime), 1, N'admin', N'帐户类型编辑', N'编辑帐户类型 白银气球卡', 145, 1, N'WN1318011106100462880001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5336, CAST(0x0000A94C0169F5CE AS DateTime), 1, N'admin', N'帐户类型编辑', N'编辑帐户类型 白银气球卡', 145, 1, N'WN1318011107140522140002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5337, CAST(0x0000A94C016A627C AS DateTime), 1, N'admin', N'帐户类型创建', N'创建帐户类型 黄金气球卡', 144, 2, N'WN1318011116408738950003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5338, CAST(0x0000A94C016D4029 AS DateTime), 1, N'admin', N'帐户类型创建', N'创建帐户类型 铂金气球卡', 144, 3, N'WN1318011179020400150001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5339, CAST(0x0000A94C016DB61B AS DateTime), 1, N'admin', N'帐户类型编辑', N'编辑帐户类型 铂金气球卡', 145, 3, N'WN1318011189085229840001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5340, CAST(0x0000A94C016E038F AS DateTime), 1, N'admin', N'帐户类型创建', N'创建帐户类型 黑金气球卡', 144, 4, N'WN1318011195695145560002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5341, CAST(0x0000A94C01750662 AS DateTime), 1, N'admin', N'帐户类型停用', N'停用帐户类型 黑金气球卡', 142, 4, N'WN1318011348853858840001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5342, CAST(0x0000A950015DE4B3 AS DateTime), 1, N'admin', N'帐户类型创建', N'创建帐户类型 礼品卡', 144, 5, N'WN1318045403520842700001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5343, CAST(0x0000A9500161B13A AS DateTime), 1, N'admin', N'初始化卡', N'初始化卡从 0000000000000001 至 0000000000000010 共 10 张', 501, 0, N'WN1318045486500454700001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5344, CAST(0x0000A9500161CD0C AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000001', 502, 1, N'WN1318045488899985890002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5345, CAST(0x0000A9500161CD16 AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000002', 502, 2, N'WN1318045488903283250003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5346, CAST(0x0000A9500161CD20 AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000003', 502, 3, N'WN1318045488906781290004')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5347, CAST(0x0000A9500161CD2A AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000004', 502, 4, N'WN1318045488909879500005')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5348, CAST(0x0000A9500161CD34 AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000005', 502, 5, N'WN1318045488913177600006')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5349, CAST(0x0000A9500161CD3E AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000006', 502, 6, N'WN1318045488916575730007')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5350, CAST(0x0000A9500161CD47 AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000007', 502, 7, N'WN1318045488919773880008')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5351, CAST(0x0000A9500161CD51 AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000008', 502, 8, N'WN1318045488923072010009')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5352, CAST(0x0000A9500161CD5C AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000009', 502, 9, N'WN1318045488926669980010')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5353, CAST(0x0000A9500161CD65 AS DateTime), 1, N'admin', N'建卡', N'创建卡 0000000000000010', 502, 10, N'WN1318045488929768190011')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5354, CAST(0x0000A9500161D479 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000001', 503, 1, N'WN1318045489533723510012')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5355, CAST(0x0000A9500161D484 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000002', 503, 2, N'WN1318045489537321450013')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5356, CAST(0x0000A9500161D48D AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000003', 503, 3, N'WN1318045489540419690014')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5357, CAST(0x0000A9500161D498 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000004', 503, 4, N'WN1318045489544117520015')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5358, CAST(0x0000A9500161D4A3 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000005', 503, 5, N'WN1318045489547515620016')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5359, CAST(0x0000A9500161D4AD AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000006', 503, 6, N'WN1318045489551113520017')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5360, CAST(0x0000A9500161D4B8 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000007', 503, 7, N'WN1318045489554511590018')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5361, CAST(0x0000A9500161D4C2 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000008', 503, 8, N'WN1318045489557909660019')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5362, CAST(0x0000A9500161D4CB AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000009', 503, 9, N'WN1318045489561107830020')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5363, CAST(0x0000A9500161D4D6 AS DateTime), 1, N'admin', N'审核卡', N'审核卡 0000000000000010', 503, 10, N'WN1318045489564505900021')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5364, CAST(0x0000A95001649C1E AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000001', 504, 1, N'WN1318045548877276800001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5365, CAST(0x0000A95001649C4D AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000002', 504, 2, N'WN1318045550262885940002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5366, CAST(0x0000A95001649C79 AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000003', 504, 3, N'WN1318045550277377690003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5367, CAST(0x0000A95001649CA3 AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000004', 504, 4, N'WN1318045550292069300004')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5368, CAST(0x0000A95001649CCF AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000005', 504, 5, N'WN1318045550306061300005')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5369, CAST(0x0000A95001649CF9 AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000006', 504, 6, N'WN1318045550320553110006')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5370, CAST(0x0000A95001649D23 AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000007', 504, 7, N'WN1318045550334645010007')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5371, CAST(0x0000A95001649D53 AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000008', 504, 8, N'WN1318045550348836900008')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5372, CAST(0x0000A95001649D7F AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000009', 504, 9, N'WN1318045550364627910009')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5373, CAST(0x0000A95001649DAB AS DateTime), 1, N'admin', N'售卡', N'售卡 0000000000000010', 504, 10, N'WN1318045550379319490010')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5374, CAST(0x0000A95001657EB7 AS DateTime), 1, N'admin', N'初始化卡', N'初始化卡从 0000000000000010 至 0000000000000015 共 5 张', 501, 0, N'WN1318045569587780960011')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5375, CAST(0x0000A950016647E0 AS DateTime), 1, N'admin', N'会员档案编辑', N'编辑会员 0000000000000001 资料', 511, 1, N'WN1318045586752731330001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5376, CAST(0x0000A9500167CE7B AS DateTime), 1, N'admin', N'会员档案编辑', N'编辑会员 0000000000000001 资料', 511, 1, N'WN1318045620080039230001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5377, CAST(0x0000A950016D2355 AS DateTime), 1, N'admin', N'删除会员卡', N'会员卡 0000000000000015 删除', 507, 15, N'WN1318045736561343430001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5378, CAST(0x0000A956009E9F8A AS DateTime), 1, N'admin', N'AddAdmissionTicketCreate.name', N'AddAdmissionTicketCreate.content', 801, 0, N'儿童套餐票')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5379, CAST(0x0000A95600AD54CF AS DateTime), 1, N'admin', N'AdmissionTicketResume.name', N'AdmissionTicketResume.content', 803, 1, N'WN1318093386476372550001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5380, CAST(0x0000A95600AEB19C AS DateTime), 1, N'admin', N'AdmissionTicketSuspend.name', N'AdmissionTicketSuspend.content', 802, 1, N'WN1318093416240693560001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5381, CAST(0x0000A95600AF0236 AS DateTime), 1, N'admin', N'AdmissionTicketResume.name', N'AdmissionTicketResume.content', 803, 1, N'WN1318093423120461600002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5382, CAST(0x0000A95600B3C16D AS DateTime), 1, N'admin', N'AdmissionTicketSuspend.name', N'AdmissionTicketSuspend.content', 802, 1, N'WN1318093526817945290001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5383, CAST(0x0000A95600B3C9DF AS DateTime), 1, N'admin', N'AdmissionTicketResume.name', N'AdmissionTicketResume.content', 803, 1, N'WN1318093527539908300002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5384, CAST(0x0000A95600B3D28F AS DateTime), 1, N'admin', N'AdmissionTicketSuspend.name', N'AdmissionTicketSuspend.content', 802, 1, N'WN1318093528280864870003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5385, CAST(0x0000A95600B42BCC AS DateTime), 1, N'admin', N'AddAdmissionTicketCreate.name', N'AddAdmissionTicketCreate.content', 801, 0, N'12312')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5386, CAST(0x0000A95600B4316D AS DateTime), 1, N'admin', N'AdmissionTicketDelete.name', N'AdmissionTicketDelete.content', 804, 2, N'WN1318093536370784270004')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5387, CAST(0x0000A95600B66E2D AS DateTime), 1, N'admin', N'AdmissionTicketResume.name', N'AdmissionTicketResume.content', 803, 1, N'WN1318093585249895970001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5388, CAST(0x0000A95600BC6EF8 AS DateTime), 1, N'admin', N'商户创建', N'创建名为 000000000000001 的商户', 14, 1, N'WN1318093716375230910001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5389, CAST(0x0000A95600FAE04E AS DateTime), 1, N'admin', N'AddAdmissionTicketCreate.name', N'AddAdmissionTicketCreate.content', 801, 0, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5390, CAST(0x0000A95600FBC17D AS DateTime), 1, N'admin', N'AddAdmissionTicketCreate.name', N'AddAdmissionTicketCreate.content', 801, 0, N'满减卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5391, CAST(0x0000A9560103F634 AS DateTime), 1, N'admin', N'CouponResume.name', N'CouponResume.content', 807, 1, N'WN1318095278938804140001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5392, CAST(0x0000A9560103FCEF AS DateTime), 1, N'admin', N'CouponSuspend.name', N'CouponSuspend.content', 806, 1, N'WN1318095279521969340002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5393, CAST(0x0000A9560104049E AS DateTime), 1, N'admin', N'CouponResume.name', N'CouponResume.content', 807, 1, N'WN1318095280178794530003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5394, CAST(0x0000A95601040D07 AS DateTime), 1, N'admin', N'CouponSuspend.name', N'CouponSuspend.content', 806, 1, N'WN1318095280896878110004')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5395, CAST(0x0000A95601041766 AS DateTime), 1, N'admin', N'CouponResume.name', N'CouponResume.content', 807, 1, N'WN1318095281781831900005')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5396, CAST(0x0000A9560105D1D6 AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5397, CAST(0x0000A9560105EF0C AS DateTime), 1, N'admin', N'CouponResume.name', N'CouponResume.content', 807, 1, N'WN1318095322029300140001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5398, CAST(0x0000A9560105FFAD AS DateTime), 1, N'admin', N'CouponDelete.name', N'CouponDelete.content', 808, 2, N'WN1318095323445423080002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5399, CAST(0x0000A95601091484 AS DateTime), 1, N'admin', N'CouponSuspend.name', N'CouponSuspend.content', 806, 1, N'WN1318095390756782660001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5400, CAST(0x0000A95601091A2C AS DateTime), 1, N'admin', N'CouponResume.name', N'CouponResume.content', 807, 1, N'WN1318095391245571500002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5401, CAST(0x0000A95601099DEF AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5402, CAST(0x0000A9560109A6F8 AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5403, CAST(0x0000A956010BDD56 AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5404, CAST(0x0000A956010BE419 AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5405, CAST(0x0000A9770172BC2C AS DateTime), 1, N'admin', N'帐户类型编辑', N'编辑帐户类型 黑金气球卡1', 145, 6, N'WN1318382818814222510001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5406, CAST(0x0000A9790125D698 AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5407, CAST(0x0000A9790125DE2B AS DateTime), 1, N'admin', N'CouponEdit.name', N'CouponEdit.content', 809, 1, N'折扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5408, CAST(0x0000A979013685F9 AS DateTime), 1, N'admin', N'CouponCreate', N'CouponCreate.content', 805, 0, N'抵扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5409, CAST(0x0000A97901369862 AS DateTime), 1, N'admin', N'CouponCreate', N'CouponCreate.content', 805, 0, N'抵扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5410, CAST(0x0000A9790136C102 AS DateTime), 1, N'admin', N'CouponCreate', N'CouponCreate.content', 805, 0, N'抵扣卷')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5411, CAST(0x0000A9790136FD52 AS DateTime), 1, N'admin', N'CouponDelete.name', N'CouponDelete.content', 808, 5, N'WN1318398793668345910001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5412, CAST(0x0000A97901370014 AS DateTime), 1, N'admin', N'CouponDelete.name', N'CouponDelete.content', 808, 4, N'WN1318398793905176420002')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5413, CAST(0x0000A97901425330 AS DateTime), 1, N'admin', N'CouponResume.name', N'CouponResume.content', 807, 3, N'WN1318399041295503370003')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5414, CAST(0x0000A97C015F2AFF AS DateTime), 1, N'admin', N'AddAdmissionTicketCreate.name', N'AddAdmissionTicketCreate.content', 801, 0, N'家庭乐园套票')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5415, CAST(0x0000A97C015F3E07 AS DateTime), 1, N'admin', N'AdmissionTicketResume.name', N'AdmissionTicketResume.content', 803, 3, N'WN1318425593004670870001')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5416, CAST(0x0000A97C015FDCE3 AS DateTime), 1, N'admin', N'AddAdmissionTicketCreate.name', N'AddAdmissionTicketCreate.content', 801, 0, N'下午茶套票')
GO
INSERT [dbo].[Logs] ([LogId], [SubmitTime], [UserId], [UserName], [Title], [Content], [LogType], [Addin], [SerialNo]) VALUES (5417, CAST(0x0000A97C015FE324 AS DateTime), 1, N'admin', N'AdmissionTicketResume.name', N'AdmissionTicketResume.content', 803, 4, N'WN1318425607092610950002')
GO
SET IDENTITY_INSERT [dbo].[Logs] OFF
GO
SET IDENTITY_INSERT [dbo].[Roles] ON 

GO
INSERT [dbo].[Roles] ([RoleId], [Name], [DisplayName], [Description], [BuildIn], [Permissions], [IsSuper], [State]) VALUES (1, N'administrators', N'超级管理员', NULL, 1, N'taskrecharging,accountquery,accountquerywithuserinfo,accountquerywithouttoken,accountapprove,accountpay,accountcancelpay,accountrecharge', 1, 1)
GO
INSERT [dbo].[Roles] ([RoleId], [Name], [DisplayName], [Description], [BuildIn], [Permissions], [IsSuper], [State]) VALUES (2, N'shopowner', N'商户', NULL, 1, N'taskrecharging,accountquery,accountquerywithuserinfo,accountquerywithouttoken,accountapprove,accountpay,accountcancelpay,accountrecharge', 0, 1)
GO
SET IDENTITY_INSERT [dbo].[Roles] OFF
GO
SET IDENTITY_INSERT [dbo].[ShopDealLogs] ON 

GO
INSERT [dbo].[ShopDealLogs] ([ShopDealLogId], [SubmitTime], [SourceShopId], [SourceShopName], [SourcePosId], [SourcePosName], [State], [AccountId], [AccountName], [Amount], [SerialNo], [DealType], [SourceShopDisplayName], [ShopAmount], [Addin], [shopId], [shopName], [shopDisplayName], [LiquidateDealLogId], [ShopRechargingAmount], [Code]) VALUES (1, CAST(0x0000A95600BC6EF2 AS DateTime), 0, NULL, 0, NULL, 1, 0, NULL, CAST(0.00 AS Decimal(18, 2)), N'WN1318093716375230910001', 103, NULL, CAST(0.00 AS Decimal(18, 2)), 0, 1, N'000000000000001', N'上海亲子店', 0, CAST(0.00 AS Decimal(18, 2)), NULL)
GO
SET IDENTITY_INSERT [dbo].[ShopDealLogs] OFF
GO
SET IDENTITY_INSERT [dbo].[Shops] ON 

GO
INSERT [dbo].[Shops] ([ShopId], [State], [ShopDealLogChargeRate], [EnableShopMakeCard], [Name], [DisplayName], [Address], [PhoneNumber], [PhoneNumber2], [Bank], [BankAccount], [ShopDealLogAmount], [ShopType], [buildin], [mobile], [mobile2], [Amount], [RecordVersion], [RechargingAmount], [BankAccountName], [DealWayId], [Description], [BankUserName], [bankPoint], [RechargeAmount], [shopDiscount]) VALUES (1, 1, NULL, 0, N'000000000000001', N'上海亲子店', NULL, N'', N'', N'中国银行', NULL, NULL, 1, 0, N'1345646', N'', CAST(0.00 AS Decimal(18, 2)), 0, CAST(0.00 AS Decimal(18, 2)), NULL, 10, NULL, NULL, NULL, CAST(0.00 AS Decimal(18, 2)), NULL)
GO
SET IDENTITY_INSERT [dbo].[Shops] OFF
GO
SET IDENTITY_INSERT [dbo].[Sites] ON 

GO
INSERT [dbo].[Sites] ([SiteId], [Name], [DisplayName], [Description], [FavIconUrl], [Version], [ServiceRetryCountDefault], [RouteUrlPrefix], [CommentingDisabled], [MixCode], [State], [SendMessageForBirthDay], [IsIKeySignIn], [ShopDealLogChargeRate], [SaleCardFee], [ChangeCardFee], [IncludeOpenSearch], [AccountNameLength], [MessageTemplateOfDeal], [MessageTemplateOfBirthDate], [PosType], [AccountToken], [MessageTemplateOfIdentity], [MessageTemplateOfRecharge], [MessageTemplateOfOpenReceipt], [AuthType], [printerType], [MessageTemplateOfUnIdentity], [TicketTemplateOfRecharge], [PasswordType], [TicketTemplateOfClose], [TicketTemplateOfDeal], [TicketTemplateOfCancelDeal], [TimeOutOfCancelSystemDeal], [TicketTemplateOfTransfer], [TicketTemplateOfSuspendAccount], [TicketTemplateOfResumeAccount], [TicketTemplateOfChangeAccountName], [TicketTemplateOfOpen], [TicketTemplateOfRenewAccount], [MessageTemplateOfPrePay], [MessageTemplateOfDonePrePay], [MessageTemplateOfTicket], [MessageTemplateOfAccountResume], [MessageTemplateOfAccountSuspend], [MessageTemplateOfAccountTransfer], [MessageTemplateOfAccountChangeName], [MessageTemplateOfAccountRenew], [Banks], [MessageTemplateOfShopDeal], [CopyRight], [MessageTemplateOfDealCode], [IsRechargingApprove], [IsLimiteAmountApprove], [DistributorDealLogChargeRate], [HowToDeals], [SmsAccount], [SmsPwd], [RetryCount]) VALUES (1, N'会员管理系统', N'会员管理系统', N'会员积分一卡通系统', NULL, CAST(2.6162 AS Decimal(18, 4)), 0, N'', 0, N'4', 1, 0, 0, CAST(0.02 AS Decimal(18, 2)), NULL, CAST(20.00 AS Decimal(18, 2)), 0, 16, NULL, NULL, N'none', N'11111111', NULL, NULL, NULL, N'password', N'default', NULL, NULL, N'none', NULL, N'1操作员:#operator# 

', N'2', 100, NULL, NULL, NULL, NULL, NULL, NULL, N'1', NULL, NULL, N'会员姓名:#username# ;
卡号:#account-name#;
卡余额:#account-amount#
卡积分:#account-point# 
【启用】', N'会员姓名:#username# ;
卡号:#account-name#;
卡余额:#account-amount#
卡积分:#account-point# 
【停用】', NULL, N'会员姓名:#username# ;
卡号:#account-name#;
卡余额:#account-amount#
卡积分:#account-point# 
旧卡号:#account-name# 
新卡号:#account-new-name# 
【换卡】', N'会员姓名:#username# ;
卡号:#account-name#;
卡余额:#account-amount#
卡积分:#account-point# 
【延期】', N'中国银行', N'1', NULL, NULL, 1, 1, CAST(0.00 AS Decimal(18, 2)), N'现金
网银
会员卡', N'dnd@dnd', N'123', 2)
GO
SET IDENTITY_INSERT [dbo].[Sites] OFF
GO
SET IDENTITY_INSERT [dbo].[SystemDealLogs] ON 

GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (1, 1, CAST(0.00 AS Decimal(18, 2)), N'1', CAST(0x0000A950016491E8 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045548877276800001', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (2, 1, CAST(0.00 AS Decimal(18, 2)), N'2', CAST(0x0000A95001649C43 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550262885940002', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (3, 1, CAST(0.00 AS Decimal(18, 2)), N'3', CAST(0x0000A95001649C70 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550277377690003', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (4, 1, CAST(0.00 AS Decimal(18, 2)), N'4', CAST(0x0000A95001649C9A AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550292069300004', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (5, 1, CAST(0.00 AS Decimal(18, 2)), N'5', CAST(0x0000A95001649CC6 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550306061300005', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (6, 1, CAST(0.00 AS Decimal(18, 2)), N'6', CAST(0x0000A95001649CEF AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550320553110006', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (7, 1, CAST(0.00 AS Decimal(18, 2)), N'7', CAST(0x0000A95001649D1A AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550334645010007', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (8, 1, CAST(0.00 AS Decimal(18, 2)), N'8', CAST(0x0000A95001649D45 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550348836900008', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (9, 1, CAST(0.00 AS Decimal(18, 2)), N'9', CAST(0x0000A95001649D76 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550364627910009', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[SystemDealLogs] ([SystemDealLogId], [DealType], [Amount], [Addin], [SubmitTime], [UserId], [UserName], [HasReceipt], [HowToDeal], [state], [SerialNo], [DealWayId], [siteAmount], [ShopName], [Operator], [accountName], [DistributorName], [DistributorId]) VALUES (10, 1, CAST(0.00 AS Decimal(18, 2)), N'10', CAST(0x0000A95001649DA0 AS DateTime), 1, N'admin', 0, NULL, 1, N'WN1318045550379319490010', 10, CAST(0.00 AS Decimal(18, 2)), NULL, NULL, NULL, NULL, NULL)
GO
SET IDENTITY_INSERT [dbo].[SystemDealLogs] OFF
GO
SET IDENTITY_INSERT [dbo].[TemporaryTokenKeys] ON 

GO
INSERT [dbo].[TemporaryTokenKeys] ([TemporaryTokenKeyId], [UserName], [Token], [ExpiredDate], [TokenKeyType]) VALUES (1, N'admin', N'6baf5483-d828-4c70-885a-9c8eec549294', CAST(0x0000A42300000000 AS DateTime), 1)
GO
SET IDENTITY_INSERT [dbo].[TemporaryTokenKeys] OFF
GO
SET IDENTITY_INSERT [dbo].[Tickets] ON 

GO
INSERT [dbo].[Tickets] ([id], [userId], [orderNo], [AdmissionTicketId], [Code], [ExpiredDate], [Price], [adultNum], [childNum], [useScope], [State], [BuyTime], [userTime]) VALUES (1, 4, N'123123122222', 1, N'1asdf12312312', CAST(0x0000A97F00000000 AS DateTime), CAST(228.00 AS Decimal(18, 2)), 1, 2, N'', 1, CAST(0x0000A97D015A14BA AS DateTime), NULL)
GO
INSERT [dbo].[Tickets] ([id], [userId], [orderNo], [AdmissionTicketId], [Code], [ExpiredDate], [Price], [adultNum], [childNum], [useScope], [State], [BuyTime], [userTime]) VALUES (2, 4, N'123123122233', 3, N'1asdf1231231233', CAST(0x0000A97F00000000 AS DateTime), CAST(216.00 AS Decimal(18, 2)), 1, 1, N'', 2, CAST(0x0000A97D015A5574 AS DateTime), CAST(0x0000A97D00000000 AS DateTime))
GO
INSERT [dbo].[Tickets] ([id], [userId], [orderNo], [AdmissionTicketId], [Code], [ExpiredDate], [Price], [adultNum], [childNum], [useScope], [State], [BuyTime], [userTime]) VALUES (3, 4, N'1231231222334', 3, N'1asdf12312312334', CAST(0x0000A97C00000000 AS DateTime), CAST(216.00 AS Decimal(18, 2)), 1, 1, N'', 1, CAST(0x0000A97D015B45F3 AS DateTime), NULL)
GO
INSERT [dbo].[Tickets] ([id], [userId], [orderNo], [AdmissionTicketId], [Code], [ExpiredDate], [Price], [adultNum], [childNum], [useScope], [State], [BuyTime], [userTime]) VALUES (1002, 4, N'2018102519570140874', 3, N'20181025195701443604', CAST(0x0000A98400000000 AS DateTime), CAST(216.00 AS Decimal(18, 2)), 1, 1, N'', 1, CAST(0x0000A9840148C5C1 AS DateTime), NULL)
GO
INSERT [dbo].[Tickets] ([id], [userId], [orderNo], [AdmissionTicketId], [Code], [ExpiredDate], [Price], [adultNum], [childNum], [useScope], [State], [BuyTime], [userTime]) VALUES (1003, 4, N'2018102519570140874', 4, N'20181025195701464604', CAST(0x0000A98400000000 AS DateTime), CAST(200.00 AS Decimal(18, 2)), 1, 1, N'', 1, CAST(0x0000A9840148C5C7 AS DateTime), NULL)
GO
INSERT [dbo].[Tickets] ([id], [userId], [orderNo], [AdmissionTicketId], [Code], [ExpiredDate], [Price], [adultNum], [childNum], [useScope], [State], [BuyTime], [userTime]) VALUES (1004, 4, N'2018102519575143924', 1, N'20181025195751446204', CAST(0x0000A98400000000 AS DateTime), CAST(228.00 AS Decimal(18, 2)), 1, 2, N'', 1, CAST(0x0000A9840149005A AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[Tickets] OFF
GO
SET IDENTITY_INSERT [dbo].[TypeNames] ON 

GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (1, 1, N'售卡', N'systemdeallogtype', 1)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (2, 2, N'售卡手续费', N'systemdeallogtype', 2)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (3, 3, N'充值', N'systemdeallogtype', 3)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (4, 4, N'储值卡押金', N'systemdeallogtype', 4)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (5, 6, N'清算支付', N'systemdeallogtype', 5)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (6, 7, N'换卡', N'systemdeallogtype', 6)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (7, 8, N'清算支付', N'systemdeallogtype', 7)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (8, 11, N'退卡', N'systemdeallogtype', 8)
GO
INSERT [dbo].[TypeNames] ([Id], [TypeId], [Text], [Category], [OrderNum]) VALUES (9, 14, N'退储值卡押金', N'systemdeallogtype', 9)
GO
SET IDENTITY_INSERT [dbo].[TypeNames] OFF
GO
SET IDENTITY_INSERT [dbo].[UserCoupons] ON 

GO
INSERT [dbo].[UserCoupons] ([id], [couponsId], [userId], [state], [receiveTime], [useTime]) VALUES (1, 1, 4, 1, CAST(0x0000A97C01799924 AS DateTime), NULL)
GO
INSERT [dbo].[UserCoupons] ([id], [couponsId], [userId], [state], [receiveTime], [useTime]) VALUES (2, 3, 4, 1, CAST(0x0000A97C01799ABB AS DateTime), NULL)
GO
SET IDENTITY_INSERT [dbo].[UserCoupons] OFF
GO
INSERT [dbo].[UserRoles] ([User_UserId], [Role_RoleId]) VALUES (1, 1)
GO
INSERT [dbo].[UserRoles] ([User_UserId], [Role_RoleId]) VALUES (3, 2)
GO
SET IDENTITY_INSERT [dbo].[Users] ON 

GO
INSERT [dbo].[Users] ([UserId], [LastSignInTime], [Email], [Password], [PasswordSalt], [State], [BirthDate], [BuildIn], [Mobile], [LoginToken], [LoginInToken], [Name], [DisplayName], [ShopRole], [ShopId], [Discriminator], [photo], [SignOnTime], [Gender], [IdentityCard], [Address], [Mobile2], [PhoneNumber], [PhoneNumber2], [isMobileAvailable], [isSale], [DistributorId], [Age], [Mark], [babyName], [babySex], [babyBirthDate], [openId]) VALUES (1, CAST(0x0000A983017427CF AS DateTime), N'1065083518@qq.com', N'zO3m/RAGD+CfsJkauzaOBMo35xX+Cpx60RyuEvqlOTo=', N'0c5e8339', 1, NULL, 1, N'', N'FuWzTxaVEHvQzq4v+bH7CNH/0BjnaFdlJ1xVyDy5qyE=', 0, N'admin', N'系统管理员', NULL, NULL, N'AdminUser', NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL, 0, 0, NULL, 0, N'广告', NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [LastSignInTime], [Email], [Password], [PasswordSalt], [State], [BirthDate], [BuildIn], [Mobile], [LoginToken], [LoginInToken], [Name], [DisplayName], [ShopRole], [ShopId], [Discriminator], [photo], [SignOnTime], [Gender], [IdentityCard], [Address], [Mobile2], [PhoneNumber], [PhoneNumber2], [isMobileAvailable], [isSale], [DistributorId], [Age], [Mark], [babyName], [babySex], [babyBirthDate], [openId]) VALUES (2, NULL, N'', N'', N'', 1, NULL, 0, N'13049313590', N'xWWQb54kgjAJE6H2r7l4T4fF0RRIxMmv5IDpUcZlvdU=', 0, N'f3c66171de1b4391a02e24ab14ad87fd', N'123', NULL, NULL, N'AccountUser', NULL, NULL, 1, N'441322199105033035', N'123', N'', N'13049313590', N'', 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [LastSignInTime], [Email], [Password], [PasswordSalt], [State], [BirthDate], [BuildIn], [Mobile], [LoginToken], [LoginInToken], [Name], [DisplayName], [ShopRole], [ShopId], [Discriminator], [photo], [SignOnTime], [Gender], [IdentityCard], [Address], [Mobile2], [PhoneNumber], [PhoneNumber2], [isMobileAvailable], [isSale], [DistributorId], [Age], [Mark], [babyName], [babySex], [babyBirthDate], [openId]) VALUES (3, NULL, N'11@qq.com', N'0PU9KSVBkaTYPXeIZYWaXek/oeDszdDAY/4arnUb5w8=', N'e6e5cc88', 1, NULL, 0, N'1345646', N'19SNJnnJfqRsyjLfighV2dIWfvQoyPz22VX50co0COU=', 0, N'shanghai', N'上海亲子店', 1, 1, N'ShopUser', NULL, NULL, NULL, NULL, NULL, N'', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, NULL, NULL, NULL)
GO
INSERT [dbo].[Users] ([UserId], [LastSignInTime], [Email], [Password], [PasswordSalt], [State], [BirthDate], [BuildIn], [Mobile], [LoginToken], [LoginInToken], [Name], [DisplayName], [ShopRole], [ShopId], [Discriminator], [photo], [SignOnTime], [Gender], [IdentityCard], [Address], [Mobile2], [PhoneNumber], [PhoneNumber2], [isMobileAvailable], [isSale], [DistributorId], [Age], [Mark], [babyName], [babySex], [babyBirthDate], [openId]) VALUES (4, NULL, N'', N'', N'', 1, NULL, 0, N'13049313591', N'IzOfu5pOu59UODw5ZY7Hf9BeMUAmSP7Q9JpukPM24DQ=', 0, N'13049313591', N'小忠', NULL, NULL, N'AccountUser', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, N'小小忠', 1, CAST(0x0000A97700000000 AS DateTime), N'123123')
GO
INSERT [dbo].[Users] ([UserId], [LastSignInTime], [Email], [Password], [PasswordSalt], [State], [BirthDate], [BuildIn], [Mobile], [LoginToken], [LoginInToken], [Name], [DisplayName], [ShopRole], [ShopId], [Discriminator], [photo], [SignOnTime], [Gender], [IdentityCard], [Address], [Mobile2], [PhoneNumber], [PhoneNumber2], [isMobileAvailable], [isSale], [DistributorId], [Age], [Mark], [babyName], [babySex], [babyBirthDate], [openId]) VALUES (5, NULL, N'', N'', N'', 1, NULL, 0, N'13690637765', N'WXqA56G13pbgY2tAIWIn4l90yx3LZcQ9GZbeypQ6H1E=', 0, N'13690637765', N'xiaozhong11', NULL, NULL, N'AccountUser', NULL, NULL, 1, NULL, NULL, NULL, NULL, NULL, 0, NULL, NULL, NULL, NULL, N'xxx', 1, CAST(0x0000A98400000000 AS DateTime), N'123123333')
GO
SET IDENTITY_INSERT [dbo].[Users] OFF
GO
/****** Object:  Index [PK__PointReb__F9EDDD312C0161DB]    Script Date: 2018/10/25 20:06:28 ******/
ALTER TABLE [dbo].[PointRebateLogs] ADD PRIMARY KEY NONCLUSTERED 
(
	[PointRebateLogId] ASC
)WITH (PAD_INDEX = OFF, STATISTICS_NORECOMPUTE = OFF, SORT_IN_TEMPDB = OFF, IGNORE_DUP_KEY = OFF, ONLINE = OFF, ALLOW_ROW_LOCKS = ON, ALLOW_PAGE_LOCKS = ON) ON [PRIMARY]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ((0)) FOR [frequency]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ((0)) FOR [frequencyUsed]
GO
ALTER TABLE [dbo].[Accounts] ADD  DEFAULT ('') FOR [useScope]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ('') FOR [name]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ((0)) FOR [adultNum]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ((0)) FOR [childNum]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ((0)) FOR [addAdultAmount]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ((0)) FOR [amount]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ((0)) FOR [weekendAmount]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ('') FOR [introduce]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT ('') FOR [details]
GO
ALTER TABLE [dbo].[AdmissionTicket] ADD  DEFAULT (getdate()) FOR [crateTime]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((100)) FOR [code]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ('') FOR [name]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ('') FOR [useScope]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT ((0)) FOR [leadersOfNum]
GO
ALTER TABLE [dbo].[Coupons] ADD  DEFAULT (getdate()) FOR [createTime]
GO
ALTER TABLE [dbo].[fz_Orders] ADD  DEFAULT ('') FOR [useScope]
GO
ALTER TABLE [dbo].[Tickets] ADD  DEFAULT ('') FOR [useScope]
GO
ALTER TABLE [dbo].[Accounts]  WITH CHECK ADD  CONSTRAINT [Account_Owner] FOREIGN KEY([OwnerId])
REFERENCES [dbo].[Users] ([UserId])
GO
ALTER TABLE [dbo].[Accounts] CHECK CONSTRAINT [Account_Owner]
GO
ALTER TABLE [dbo].[PosEndPoints]  WITH CHECK ADD  CONSTRAINT [PosEndPoint_Shop] FOREIGN KEY([ShopId])
REFERENCES [dbo].[Shops] ([ShopId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[PosEndPoints] CHECK CONSTRAINT [PosEndPoint_Shop]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [User_Roles_Source] FOREIGN KEY([User_UserId])
REFERENCES [dbo].[Users] ([UserId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [User_Roles_Source]
GO
ALTER TABLE [dbo].[UserRoles]  WITH CHECK ADD  CONSTRAINT [User_Roles_Target] FOREIGN KEY([Role_RoleId])
REFERENCES [dbo].[Roles] ([RoleId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[UserRoles] CHECK CONSTRAINT [User_Roles_Target]
GO
ALTER TABLE [dbo].[Users]  WITH CHECK ADD  CONSTRAINT [ShopUser_Shop] FOREIGN KEY([ShopId])
REFERENCES [dbo].[Shops] ([ShopId])
ON DELETE CASCADE
GO
ALTER TABLE [dbo].[Users] CHECK CONSTRAINT [ShopUser_Shop]
GO
