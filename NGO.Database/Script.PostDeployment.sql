/*
Post-Deployment Script Template							
--------------------------------------------------------------------------------------
 This file contains SQL statements that will be appended to the build script.		
 Use SQLCMD syntax to include a file in the post-deployment script.			
 Example:      :r .\myfile.sql								
 Use SQLCMD syntax to reference a variable in the post-deployment script.		
 Example:      :setvar TableName MyTable							
               SELECT * FROM [$(TableName)]					
--------------------------------------------------------------------------------------
*/
:r .\PostDeploymentScripts\Roles.sql
:r .\PostDeploymentScripts\Users.sql
:r .\PostDeploymentScripts\UserRoles.sql
:r .\PostDeploymentScripts\UserDetails.sql
--:r .\PostDeploymentScripts\OrganizationChapter.sql
--:r .\PostDeploymentScripts\Organization.sql
 --Scaffold-DbContext "Server=.;Database=NGO;Trusted_Connection=True;" Microsoft.EntityFrameworkCore.SqlServer -NoOnConfiguring -Namespace "NGO.Data" -ContextNamespace "NGO.Repository" -ContextDir "D:\Aykan\Projects\NGO New\NGO\NGO.Repository\Infrastructure" -OutputDir "D:\Aykan\Projects\NGO New\NGO\NGO.Data\NGO\Entites"  -force