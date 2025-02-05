
SET QUOTED_IDENTIFIER ON
GO

CREATE VIEW [dbo]."vw_Bank_vwCashFlow"
AS
SELECT "CFWId", "CFWName" FROM "OCFW" WHERE "Postable" = 'Y'
GO


