
/* =============================================
  Author:            Mihaly Szijjarto (Orchard Rock Development)
  Create date:       17/12/2020
  Description:       Returns Fees to be quoted on the client care letter

Test:  
       SELECT * FROM dbo.fn_OR_RESI_Get_CCL_FeesList('TES000000000018',2,'Our Costs,Land Reg Fee (Portal)','')
	   SELECT * FROM dbo.fn_OR_RESI_Get_CCL_FeesList('TES000000000018',2,'Our Costs,Land Reg Fee (Portal)','Total')
      
              
History: 
       When         Who					What
	   17/12/2020	M Szijjarto			Initial
	   21/12/2020	P Taylor			Included @NotBuyingWithMortgage condition 
	   21/12/2020	P Taylor			Included @Tenure condition, only returns Leasehold fee if case is a leasehold
	   22/12/2020	P Taylor			Added option of returning multiple fees and a Total option  
       
============================================= */

ALTER FUNCTION [dbo].[fn_OR_RESI_Get_CCL_FeesList]
(
       @EntityRef varchar(15)
       ,@MatterNo int
	   ,@FeeDescList varchar(2000) = ''
	   ,@ReturnType varchar(200) = ''   --List, Total
)
RETURNS @tblFeeList TABLE
(
       FeeDesc Varchar(200)
	   ,Suffix Varchar(200)
       ,Amount money 
       ,VAT money
	   ,AmountFormatted varchar(20)
	   ,VATFormatted varchar(20)
)
AS
BEGIN
	DECLARE @CaseType varchar(20)
			,@Delimeter char(1)
			,@NotBuyingWithMortgage char(1)
			,@Tenure varchar(30)


	SELECT @CaseType = dbo.fn_OR_RESI_GetMatterDetails (@EntityRef, @MatterNo, 'Case Type')
	SELECT @Delimeter = CASE WHEN charindex('|',@FeeDescList) > 0 THEN '|' ELSE ',' END
	SELECT @NotBuyingWithMortgage = isnull(Mortgage_Not_Required,'N') FROM usr_OR_RESI_MT WHERE EntityRef = @EntityRef AND MatterNo = @MatterNo
	SELECT @Tenure = isnull(Tenure,'Freehold') FROM Usr_OR_RESI_MT_Property WHERE EntityRef = @EntityRef AND MatterNo = @MatterNo

	
	IF @CaseType = 'Sale'
	BEGIN 
		INSERT INTO @tblFeeList (FeeDesc, Suffix, Amount, VAT)
		SELECT FeeDesc, Suffix, Amount, VAT
		FROM dbo.fn_OR_RESI_GetFeesList (@EntityRef, @MatterNo) 
		WHERE FeeDesc IN (SELECT Data FROM dbo.fn_OR_RESI_Split(@FeeDescList,@Delimeter))
		
		UPDATE f SET Amount = mf.Amount
		, VAT = mf.VAT
		FROM @tblFeeList f
		INNER JOIN Usr_OR_RESI_MT_Fees mf on mf.FeeDescription = f.FeeDesc
		WHERE mf.EntityRef = @EntityRef
		AND mf.MatterNo = @MatterNo
	END

	IF @CaseType = 'Purchase'
	BEGIN 
		INSERT INTO @tblFeeList (FeeDesc, Suffix, Amount, VAT)
		SELECT FeeDesc, Suffix, Amount, VAT
		FROM (
			SELECT FeeDesc
			, Suffix
			, Amount 
			, VAT 
			FROM dbo.fn_OR_RESI_GetFeesList (@EntityRef, @MatterNo) 
			UNION SELECT 'SDLT' FeeDesc, '' Suffix, SDLT_Amount Amount, 0.00 VAT
			FROM Usr_OR_RESI_MT_Property	
			where EntityRef = @EntityRef
			AND MatterNo = @MatterNo
		) _Fees
		WHERE FeeDesc IN (SELECT Data FROM dbo.fn_OR_RESI_Split(@FeeDescList,@Delimeter))
		
		UPDATE f SET Amount = mf.Amount
		, VAT = mf.VAT
		FROM @tblFeeList f
		INNER JOIN Usr_OR_RESI_MT_Fees mf on mf.FeeDescription = f.FeeDesc
		WHERE mf.EntityRef = @EntityRef
		AND mf.MatterNo = @MatterNo

		UPDATE @tblFeeList SET FeeDesc = 'Land Registry Fee'
		WHERE FeeDesc = 'Land Reg Fee (Portal)'

	END

	--Duplicate entries where applicable e.g. per person fees
	INSERT INTO @tblFeeList (FeeDesc, Suffix, Amount, VAT)
	SELECT fl.FeeDesc, _clients.Salutation, fl.Amount, fl.VAT
	FROM @tblFeeList fl
	CROSS APPLY (SELECT Salutation
					FROM std_MatterContacts 
					WHERE EntityRef = @EntityRef 
					AND MatterNo = @MatterNo
					AND EntityTypeRef in (SELECT EntityTypeRef FROM dbo.fn_OR_RESI_GetEntityTypeList('Client'))) _clients
	WHERE fl.Suffix in ('Per person','Per Client')

	DELETE FROM @tblFeeList
	WHERE Suffix in ('Per person','Per Client')
	

	--Return empty/zero (0.00) row if no fees found
	--IF (SELECT Count(*) FROM @tblFeeList) = 0
	--BEGIN
	--	INSERT INTO @tblFeeList (FeeDesc, Amount, VAT)
	--	SELECT '', 0.00, 0.00

	--	UPDATE @tblFeeList
	--	SET AmountFormatted = ''
	--	,VATFormatted = ''
	--END

	--Remove items not applicable to the case
	IF @NotBuyingWithMortgage = 'Y'
		DELETE FROM @tblFeeList
		WHERE FeeDesc LIKE 'Buying With% Mortgage' 

	IF @Tenure = 'Freehold'
		DELETE FROM @tblFeeList
		WHERE FeeDesc LIKE '%Leasehold%' 

	--ReturnType = Total
	IF @ReturnType = 'Total'
	BEGIN
		INSERT INTO @tblFeeList (FeeDesc, Suffix, Amount, VAT)
		SELECT 'Total', '', SUM(Amount), sum(VAT)
		FROM @tblFeeList fl

		DELETE FROM @tblFeeList
		WHERE FeeDesc <> 'Total'
	END

	UPDATE @tblFeeList
	SET AmountFormatted = '£' + format(Amount,'N2')
	,VATFormatted = '£' + format(VAT,'N2')


    RETURN

END

	


