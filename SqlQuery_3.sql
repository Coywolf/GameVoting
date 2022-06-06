﻿IF object_id(N'[dbo].[FK_dbo.Option_dbo.OptionSet_OptionSetId]', N'F') IS NOT NULL
    ALTER TABLE [dbo].[Option] DROP CONSTRAINT [FK_dbo.Option_dbo.OptionSet_OptionSetId]
IF EXISTS (SELECT name FROM sys.indexes WHERE name = N'IX_OptionSetId' AND object_id = object_id(N'[dbo].[Option]', N'U'))
    DROP INDEX [IX_OptionSetId] ON [dbo].[Option]
CREATE TABLE [dbo].[Game] (
    [GameId] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](100) NOT NULL,
    [BggId] [nvarchar](10),
    [MinimumPlayers] [int],
    [MaximumPlayers] [int],
    [Description] [nvarchar](max),
    [CreatedBy] [int],
    CONSTRAINT [PK_dbo.Game] PRIMARY KEY ([GameId])
)
CREATE INDEX [IX_CreatedBy] ON [dbo].[Game]([CreatedBy])
CREATE TABLE [dbo].[GameSetGame] (
    [GameSetGameId] [int] NOT NULL IDENTITY,
    [GameSetId] [int] NOT NULL,
    [GameId] [int] NOT NULL,
    CONSTRAINT [PK_dbo.GameSetGame] PRIMARY KEY ([GameSetGameId])
)
CREATE UNIQUE INDEX [IX_GameSetGame] ON [dbo].[GameSetGame]([GameSetId], [GameId])
CREATE TABLE [dbo].[GameSet] (
    [GameSetId] [int] NOT NULL IDENTITY,
    [Name] [nvarchar](100) NOT NULL,
    CONSTRAINT [PK_dbo.GameSet] PRIMARY KEY ([GameSetId])
)
ALTER TABLE [dbo].[EventOption] ADD [GameId] [int]
INSERT INTO dbo.GameSet (Name) SELECT Name FROM dbo.OptionSet
INSERT INTO dbo.Game (Name) SELECT Name FROM [dbo].[Option]
INSERT INTO dbo.GameSetGame (GameSetId, GameId) SELECT gs.GameSetId, g.GameId FROM [dbo].[Option] o INNER JOIN dbo.OptionSet os ON o.OptionSetId = os.OptionSetId INNER JOIN dbo.Game g ON g.Name = o.Name INNER JOIN dbo.GameSet gs ON gs.Name = os.Name
INSERT INTO dbo.Game (Name) SELECT o.Name FROM dbo.EventOption o LEFT JOIN dbo.Game g ON g.Name = o.Name WHERE g.GameId IS NULL
UPDATE dbo.EventOption SET dbo.EventOption.GameId = dbo.Game.GameId FROM dbo.EventOption INNER JOIN dbo.Game ON dbo.Game.Name = dbo.EventOption.Name
ALTER TABLE [dbo].[EventOption] ALTER COLUMN [GameId] [int] NOT NULL
CREATE INDEX [IX_GameId] ON [dbo].[EventOption]([GameId])
ALTER TABLE [dbo].[EventOption] ADD CONSTRAINT [FK_dbo.EventOption_dbo.Game_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Game] ([GameId])
DECLARE @var0 nvarchar(128)
SELECT @var0 = name
FROM sys.default_constraints
WHERE parent_object_id = object_id(N'dbo.EventOption')
AND col_name(parent_object_id, parent_column_id) = 'Name';
IF @var0 IS NOT NULL
    EXECUTE('ALTER TABLE [dbo].[EventOption] DROP CONSTRAINT [' + @var0 + ']')
ALTER TABLE [dbo].[EventOption] DROP COLUMN [Name]
DROP TABLE [dbo].[Option]
DROP TABLE [dbo].[OptionSet]
ALTER TABLE [dbo].[Game] ADD CONSTRAINT [FK_dbo.Game_dbo.UserProfile_CreatedBy] FOREIGN KEY ([CreatedBy]) REFERENCES [dbo].[UserProfile] ([UserId])
ALTER TABLE [dbo].[GameSetGame] ADD CONSTRAINT [FK_dbo.GameSetGame_dbo.Game_GameId] FOREIGN KEY ([GameId]) REFERENCES [dbo].[Game] ([GameId])
ALTER TABLE [dbo].[GameSetGame] ADD CONSTRAINT [FK_dbo.GameSetGame_dbo.GameSet_GameSetId] FOREIGN KEY ([GameSetId]) REFERENCES [dbo].[GameSet] ([GameSetId])
INSERT [dbo].[__MigrationHistory]([MigrationId], [ContextKey], [Model], [ProductVersion])
VALUES (N'201606261946084_Games', N'GameVoting.Models.DatabaseModels.VotingContext',  0x1F8B0800000000000400ED5D596FDCC8117E0F90FF3098A724F06A241B011243DA852D1F51B23EE0B1377913E899964C8443CE921C4542905F9687FCA4FC8534D93CFAA86AF64572B408165858D3DD5F575757551F55ECFAEFBFFF73FEC3FD2E59DC91BC88B3F462797672BA5C9074936DE3F4F66279286FBEFBC3F287EF7FFDABF3D7DBDDFDE2A7B6DEB3AA1E6D991617CB6F65B97FBE5A159B6F64171527BB7893674576539E6CB2DD2ADA66ABA7A7A77F5C9D9DAD08855852ACC5E2FCD3212DE31DA9FFA07F5E66E986ECCB4394BCCBB624299ADF69C9BA465DBC8F76A4D8471B72B17C4BFFF9535652FA4E58E5935751197D8D0AC2FE5C2E5E247144E95A93E466B988D2342BA39252FDFC4B41D6659EA5B7EB3DFD214A3E3FEC09AD771325056946F3BCAF6E3AB0D3A7D5C0567DC3166A7328CA6C670978F6ACE1D44A6EEEC4EF65C749CACBD794E7E54335EA9A9F17CBD777242D970BB9A7E797495ED51AE6F5498DF064D1D77BD2C90815A5EABF278BCB43521E72729192439947C993C5C7C3D724DEFC853C7CCEFE4ED28BF490243C9D94525A26FC407FFA98677B92970F9FC80D4FFDD576B958898D5772EBAEADDC900DF22A2D9F3D5D2EDE5332A2AF09E9048263C8BACC72F296A4248F4AB2FD189525C9E97C5E6D49CD528504A9C3CB9C54ED5E3E0C75A987A9DA0E93ADC7A8FEDF225065A033B65CBC8BEE7F24E96DF9ED62F9F4F754AFDFC4F764DBFED2A07E49636A1468A3323F10EB5EAF8A8F797C4779D076FD32CB1212A5D640EB32CACB571C50F5EFCFF1CE9EA4D7E916C6919ABD8FEEE2DB5A0AA049CDF2E5E21349EAF2E25BBC6756E7845A9A9C56BF891372FDE11F29D9D622474DD39B3CDB7DCA2A22912AD79FA3FC9650A5FC9CE9EBADB343BEB120B752D10224F6AF59BAA53A5B55B86E2C424FA652A810A8D6B025ED1DD97DA50820713522ABA012A7142AC4A9356C89FBB0AF29C289631510E2F8429838A1862D7195ECE29455A5D7AAE4C965305D7C0588ACF355BF9C6817194E80BD961A0E67A605A7A2C065BD69DB4DB5DC54FD0DD8F967A398F9669D73B3CFA890733A5C49F8B09DA8C68F9989AA4C6B25EA0AD64682B7F161D7839694A1F5A01D93938E720CF0DF0E329C99749475EEA2A57DCBA9F4D4701F3AACECBE187FCEE2D4556D25A83F45C52B7243F29C6CADF77A7A0B10627B202B13BE8130258DD91A5F83A4A34BB058A66451BD43767B3574557CDDEABB441757061B4ABE82D7CEA043F3B73915CA4C16A7EADAC5DEB4EDA6B2366CA3E96B2A4CADE4C0316E4387E207F16354945FF6DB11F61BAD5EF82A0FA8D49076D91D45064F228DEEC32791BA507B126135FC15BBA5D657B519CE4CCADD2B8DAD7A9BAADB916D272A2E8F7CC774761AE6F0E1BA5D303FB0831A0C1EE96D6E610629639560C2EA6B161D5D7585C0DB0573C3A2A34C343D4E868571C6C3A25415663225AD66D91A12338D0C674626D25EA9D797B7B7FD20C16E8D7A1DD8BCC469BC3BEC3E26D1437DDF28B074A06974EFDAF415293679DCAC86E800E93F038C10F33484B8D4AE2D4B57A1B702FCEF8A01100A5DACE59A20772A4D61DD816C31E53265C3A354F0DAEF7068DED6A9C199D1483514B8DA2AAEF95426ABE97696BD8FFD526F25B8903E81926DA9533ACAAE1B5F9142162BC094A9290DA14921B4685E0DF2D09E5FE062EFE095349445443F2449759245CEB1E9258F1CCEFFB79E0E27D849622BC6715DE9B653BCDF1CD85501C58AB043756CD703FCB06C1917A023CEEDB0DC6DB671E258156519554BB1B005BE8AD7CA25E085B0170C69268BC13A77B1197DCB29777FA378D1064EAC59946F2DDBCCE2955F53C3E079E91F277119E50F01AEED2FB3380D00C37424043DF15DBC0932AEDD8EE49B384A0280BD3DC4C93600CE7A1393744342785B48E4C86FD4BAD7FAA3B3ED7585E2BA5B0514F32E56C02CBC54CB256E2ECC0A842C90D022E5ED7D16B165FFB35A3A401CEA83B65D1ED96484581E19D24CCB6367FB6D574778D198EF80172ADED7670F67ABE788A822D6C0DD81C9822B7DDD9755DD99A4B48D17B71552B338F3477709A13A03A0F56CD00D60DFC8CC0160AA8A56073B8D53CF343A1874E941E1C3269AF6A22832BA49AAE851430F9B53A638BED7E9763118CEC5782A855D51EE52C58AF75495282517CBDF29BCD36177BB09095B463D53503F55F16F95E447C925653B55E9382D556D8DD34DBC8F922102A486A0A2235FC254D3D0F52397BC227B92560A3AC45D4F02BA7E242334C4A5F315272BE622546F754C6659DCF504152031C6B8871622E14797229E0A933984A3DB9D6588E7AF5FF75349101FD7A59D6630C84B922016D668293F60E8A5916886941F800A9309C422AFED2508E0AF2F0153C990105BA49D6B38D0489AEC3686CE528EE0AF8E265CC82002265DC820EE1EFF4226F802B019863D02FDE4B29B08738901BD0766ABD6E9C94920A981889868D182F869D235E7789A4D58843002DD04C33105A2D074112C76B203C7D9E8E531A0D440DD9B4C1FEC13B5961C88B17EDD4F273A8D6F7D60726547BB2234C3938B6122D2622C883E42230ECB74CAC0C80E17A11139E0DDFDC4FB1BADBDC1C39583EE6E66B03758F713D91B8CB1476F6FD4207293099622CA8D8447677B349FC1381DDEBC454818A0C924625FA3384B91C0065F0A261024EC3B6A6CCA073FAA76BC9C19EEC1F4D41548AC06063AD1567A80198F61570D45656173AF0DD1EAE75D8823345FEB74C15D935E2A6A089948AE348C7E6C32A5BF19D244D5059527BB9BA170C77C9486A92E8750061FFFE510EC2D1F986DCC75AE48531B986130F3439D00622505478EB2046A476C32BD4850878B7CC11CF124623A19E3639A06E61E0C70729E782D3C2E57231EF9703A263AF4E17C3EFA631F10846636DD986F36903439F867832F810029D36EA60056FB1130D53D021FB7A23DEA83412CD25687C57859DE2140C12F139DF3B081994C1D1C0E667F77000CDFAFFBE092F3BA8E3DA26D4ADA82E4FD2307717A5BFD4AEEA12F0CA96837B17D45136B254B4185BA26A5F8E5CCEB2ED0499C7F4586C4D682B1513084D20124212C00A6A62D354162975F300E2B334169AFE7609CB6740089ADEB0A04FBD9A06DE7480021BA5233241C651041D8A6282842A91952BB0262586DB9195ABBF7C7D0DA7293796706159E755626A1702A0C8A74FBE919570F7DFB4DB62C26B183DD28244552CC9449B0A00CA6C6DE8AC3B564058BE3D77342DD66A1B40BDB2C4F3E087B2A0E4B67C81C9921BCBF8530038DE55207004573C9CC80EC9E1E099308C4123BB2427CCA08E1051E9384BA47F42A025BEF01B0115544FC8456E5027E992BD00CDEE372E442061A07185D1794F80F64E4788C88423C18252271005935F5580033C0E5CE990BED3301280BA05807886629DA411DBCC9C0A5F006330E7ADA004C04F46E7B54693111703100238B80EA5D1E6003E0824669179DD09E8C10BDCE268B8C033BD0B7A655A618B95385D10C3954CDAC9D11E4880B06F8F482CA9E415FA0B137901B8366B76FECFE1B6D4D019E7CD0B3656853813BB43C5932FAA602F938116587CE2F63E19951D9029FBE2CFC302A24723C746692F00D34CA21D4AB60EA57301D88A923C14C04BDD9829DDA86AEC74D2FC803B065C2D39BF0D921B24EA357BC6697BCB23580EE1DCC6E753DEC4AFB0564770BD9959DAF5852B3E687F31592FDECFC5DB4DFC7E92D970DADF965B166A9D02EBF5BDB6705DB318CD546E0AD7C67DAF544D79DE89648A5D57B175BF226CE8BB2FDE679B9B8DCEE946AE29D2B727FD4F6C55FABAA93D5DE28B5B5AB7FF76780E10FB1814BEA06EA0D1DE18ED6A8074B943957DA2DAAB4745112E5D8F355975972D8A5034103380E17AFC22369C25870ACF6D69D07C26EE27114F685368FF11E5CAC71042E03180FC3FD6C8EC52501E3B1B89FCDB1BA3460C29CB53FAA38E72B4968146F8622A992C590C5DE482978431C543534C0060AA26D8DB1BC7521F21CC7DC8A7A14552CFB5FADD54D1503A1E0684481BF460C6F256160535B89B5C658DF7FB2C9F31DFF9013470A657BC308279F508747E27F37471372EAF07042C17109687D99115E3C215853E184DB624C6FD3B3F0FC8653BDE850FA78791E078FA2C791C2A94AF38089B074B29FCC3194EC2B3C9A52785CA2D95CDA85174E18D8543CB1D6E38B5628DBD9069BF13058001A8E62BED79C4988900B0C0FE981100DC4066E762CB3832134991E7888E6270B63282572104CA25466812AE5781050A5327354E1F5271E522818F77438A3A6B4DEAEE00A83001BEA0DDA5AA73EEB3EB581AC456B2C698201268237A6F19D57204611067741B01682201376F4EB1C7F271E74C234C00693A66D3DEEE2778C177C8FECF642F4508C215630B4B96061EDB109E81F3CE7B98F3DA0AE430A23A4616E30BAEF90847D1AF671128EE3BF5F64CF970BA7D5FA179BDDA2F07CB9B859148A2CD4AE7FC75C50BAFE67732CE131731E4D28B0A08D7BD35C208EFBDD66A4D2CBE6E278A5420B69E71E3917249EFBDDE652837FEA5CBCDBE04B2CAE38F827CF85EB0DBEE0D80C6B132D30866185A1CD0D2BD6FE784CD09CB752B5933AFC9D14046B7A2305B73D764FE33BD030BF73B0CBFD63CFD24D803592EB1DC0A802A94428C855BADE9B5FBABFBB0885263A40085BA879500521D4632F9A4805395C8055592E28A3EEE26D152AB07E284AB2AB85F864FD737299C475C0455BE15D94C637A428D993E8CBA7A7674F978B17491C152C72A40984782E7FD665141971F6AC8A8C20DBDD4A6E6E1F5F51A114C55678B11D7961DEE55177E4B831FCAA7BD7907D961357CC1D7CB8DD2D8B569F8CABEE45F9D2EE8AAE07F717CB7FD6AD9E2FAEFE76DD357CB2F890D3D97EBE385DFCCBBA7BF1E17AD3BE592BAF8E998D62DDA67751BEF916E5EAE3EE96A05C6C0443FE1A97F62960FAA0080652B9654AA3AC6658A4048EA33E506F9C71018D2430D30AF88033AC1462DEA75174A20F49D0C8C7337BF10032D619CEAD5D1A0CF0456CA349C19C93C3D3D2B71C7162208368602EEA66D5A47616E3ECC9E2AAF892C63F1F688DCF54071C2C0820884EB43C0D400B1FA5E06934840805532B66279FAA5BDF4C3A6167FEB06CB6ED4694CCDE896B270EAC1D539EA0D209EAA32B4121445448C25253639BD04C8E5118D182429E7D3319C5BCF9C3520A8AD0315950CF1D577B2F6AD7336B35EE56AF4E5714467A80079826CCF73B8AC804609F0ADAC40F6851F5FB54C0E4496104A0A931029222079C81803452ED587FB38BEE7F6B8B37DE61CDE31C80FAE6ADB2AFFB2800D77C443DE05CC6F626AC2131E812EF6E50656AECD7775BE9F0900C0FA9387ECBE894DAFE97B5BE8CB32599D79C1ECB7580C6413E7556F1D16CF22C7B4AB7C33F6BE523B252365AD38E9B663E3D8F7121C6E7397739128259CE5D80941CE72E20408673275A94FCE66E2302B39BBB40A9B9CD5D50A0CCE64ED7006A5EF36118C7A4D70E0633549EE979362CC34E0F8784CC0E5C84BDD0B679908F75D3071E5D75126D7868F585D01C577552627E76F4CE25EC9213117CFDCE3D81ACFB23B07324067E04AFE4EB9F42849F1D0A9107663E69D07FC16DEA2A1D5524E67F34DA38B7AFDD140E4A047BD34B061B3BBF94C5D484CDDAFB0BCCD31B6091F8202C82F6F9EF1ECB22817EFF7A6C8B04FEA29BDF0231902EE52DB7D9B348D5FB585605E47B1EB5AFB9F331E95F45B59D308374A720CE7AF42CA733E4C13DFEA4249AF760D5890B94EE76A6F9873F8AB4708D8C2E0568AFC794CFD6DB0ACCB70998D20AD86E0166B50203CF01FB4DDE919E0A2CE6265CA259E353C1E3CD2C1BECFA608E6CB1935E193C8ADDA14D06D8C05220BC42CBC15965FC7C2C12A17F05E0B8E522C8BD815972CD198461DAAB035B3998F7EAC0230B6BE8B4ABED2BCF2AEA747956A74EAC8A3EEE60D5EB516652B551F4A3978E290F1EF6A231EBD1C3222B6AF82BC8798563BE5D868570CCEFACAA66C538C369A09CA6D367309D3467E9E3C850AA3EAA2FCF59F3313F2F2D780652F68DFDC572FB35A3D3CBC22BE0340632B0457252A89391B297A2E3099BDE14ED2674F653B4A310E951217030BD0984BA364B9C8AF581653843BAD26556D57431086F917615EA66C4BCAC9AEEC649DDAAE9306C765754A659B1D4C984C95F8793B9A86D44B3A43C73AF0414CE9DDC1530DB56A9D8261C74E024AE76446337F272EB4019F9D4889310A95A5D045A5C9794B7C7FD872ADC9ABAA762F51564DEE4F74F658719DE3A6CCE55FB14B3D2E2AC3C691C6C98C192AABAE6511D79988A07D82F7DAAFD6C4EA393AAAF335C7E54F794A8A35ADCA972A0BAAFB28F27AFA9EF70815DB8F28E73D8A1A36BAC4DE652973576D2A18E9C99D42F15A97810029E570EC68611728FDACCDF510C3E4886D1407A3EC1E0954BD24079445D33874E9527547D6DF37CF5E990569F78B3BF5E9122BEED21CE29664A36C2DD6357E72ABDC9DA2B5089A2B68AF20854196DA3327A9197F14DB42969F18614459CDE2E173F45C9A162073D6D6DAFD20F87727F28E990E9E92B11FCF5D555AAAEFF3A19AA48F339DB5114218640C98CABAFE23FA42FABCF503BBADF009F262210D51D6DF30160359765F521E0ED4387F43E4B0D811AF67557CB9FC96E9F50B0E243BA8EEE880B6D54557F24B7D1E6E163F3682A0E323C1122DBCF5FC5D16D1EED8A06A36F4FFFA432BCDDDD7FFF3FF134873C4FE40000 , N'6.1.1-30610')

