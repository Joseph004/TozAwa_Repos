using System;
using System.Net;

namespace Tozawa.Language.Client.Helpers;

public static partial class SystemTextId
{
    public static Guid Name => Guid.Parse("f562400a-d318-4ee3-bf9f-8e8879214b6b");
    public static Guid Success => Guid.Parse("06e63f65-c762-4650-ba32-b177aa26459f");
    public static Guid Forbidden => Guid.Parse("edd0ed36-48a1-40d5-b15c-4f72ecd03085");
    public static Guid Unauthorized => Guid.Parse("1c670beb-c72a-41a3-bd15-8d0dae88c0a1");
    public static Guid NotFound => Guid.Parse("ae2a4ae2-0b77-4038-80f1-8e6811ef184a");
    public static Guid DefaultError => Guid.Parse("2ab768fd-4055-4cee-a6a6-0ec842bd6679");
    public static Guid Login => Guid.Parse("195E79A9-F767-4246-8D30-BB8A834B07C0");
    public static Guid Cancel => Guid.Parse("d9cbe699-7270-4b9d-ac74-377f82cbc85c");
    public static Guid Close => Guid.Parse("36824adb-80d4-480c-b8fb-474e445673cb");
    public static Guid Email => Guid.Parse("cd04b055-c8c3-46c1-aa5d-d962a30f15f1");
    public static Guid Password => Guid.Parse("693242ad-7369-4f5d-ad44-881240770d31");
    public static Guid Error => Guid.Parse("f912da13-4134-4b97-829a-e012b31416ed");
    public static Guid Processing => Guid.Parse("e840168a-123d-441d-855c-69ae27d18078");
    public static Guid UserName => Guid.Parse("440a071a-bbfd-4175-a05c-73780913cfd0");
    public static Guid Administration => Guid.Parse("2512c06a-e8e2-4f28-bd56-2dc2b4e74c80");
    public static Guid Logout => Guid.Parse("d2ff9654-fd31-4cbf-8c52-6ae071d38371");
    public static Guid Setting => Guid.Parse("ad71d9fe-67aa-4b8a-8661-1c603e4ec35a");
    public static Guid StayLogIn => Guid.Parse("7f1a951d-82e3-4bda-aa5b-011f0552cd22");
    public static Guid YoureAboutToGetLogout => Guid.Parse("6c12b3d5-3661-42ff-b97a-74e0a5b2102e");
    public static Guid Member => Guid.Parse("c43eaa7e-3016-420f-8c6a-70c26d82a223");
    public static Guid Members => Guid.Parse("db52be9f-8189-42c2-8dd3-1609caa03173");
    public static Guid Organization => Guid.Parse("723f9da4-2551-4120-871a-1fc96554a3d7");
    public static Guid Organizations => Guid.Parse("3cf9101d-15bb-4481-9094-8009b7c92dff");
    public static Guid Hello => Guid.Parse("7c06ef7d-12f5-4875-8335-fdfd41c55b36");
    public static Guid EntityDeletedSuccess => Guid.Parse("ed26cfa1-ee01-4422-9f66-c5512c5bfd1a");
    public static Guid EntityCreatedSuccess => Guid.Parse("7c75d4d2-5df5-4e1e-8a83-00d7869f7877");
    public static Guid EntityCreatedError => Guid.Parse("9e785ba9-d4e0-4d97-8b7a-a41ef787aa0f");
    public static Guid ImportSuccess => Guid.Parse("3948f384-3897-4fd6-8371-803801acd74b");
    public static Guid ImportError => Guid.Parse("492ad18c-103c-42b9-8df1-289b8d8df62e");
    public static Guid Sorry => Guid.Parse("5608eef3-268b-49fe-9ff9-f293d8677785");
    public static Guid YoureNotAuthorizedToReachThisPage => Guid.Parse("4c4a82df-5135-442e-913a-108b6dc4626a");
    public static Guid YouMayNeedToLogInAsADifferentUser => Guid.Parse("f8eb2e15-fe92-4106-9138-86c90b49f5af");
    public static Guid EmailNotExists => Guid.Parse("ef0b6283-c7aa-48cb-90d3-8ca679b6e6b1");
    public static Guid PassWordWrong => Guid.Parse("e1257997-a841-4a76-b4c5-f059750689f9");
    public static Guid UserNameNotExists => Guid.Parse("da87dc9d-317c-445c-b341-98aee877986a");
    public static Guid NotRootUser => Guid.Parse("ac1aab8d-27ea-4a25-958f-11ffd9c7a62f");
    public static Guid UserAlreadyExist => Guid.Parse("c8642095-235d-4a49-b01c-32e516809ee3");
    public static Guid UserCreationFailed => Guid.Parse("58bcd41e-e444-4156-826b-234ed2ac6a93");
    public static Guid UserCreationSuccess => Guid.Parse("aff944b3-366e-490c-abbd-07aa31e5fab4");
    public static Guid Home => Guid.Parse("778aaa56-c345-4336-93ea-1d866d243a49");
    public static Guid SearchText => Guid.Parse("b752a56a-f067-4937-ae38-ac8d496d6413");
    public static Guid Restore => Guid.Parse("c14a265b-0c57-4594-b464-6d22bab412f7");
    public static Guid Delete => Guid.Parse("e2d165ce-741d-4388-a7fa-0e104dc94504");
    public static Guid RowsPerPage => Guid.Parse("79396871-e984-4746-953a-4dd28cd8c445");
    public static Guid ShowDeletedItems => Guid.Parse("10cba804-b8b6-49e2-b3b6-b96fdea1035f");
    public static Guid Add => Guid.Parse("ffeee73c-ca11-4b14-bc87-f991da14edeb");
    public static Guid FirstName => Guid.Parse("eaaadc84-0de1-4a6d-ba48-c394b5e484bf");
    public static Guid LastName => Guid.Parse("84e48e74-3760-4f08-8c75-e98bb728b9ee");
    public static Guid Description => Guid.Parse("2f21ae24-6168-4047-8ab2-db1849b3868e");
    public static Guid NoMachingRecords => Guid.Parse("51353e2a-00db-4b36-be2a-edb59634bbb8");
    public static Guid Loading => Guid.Parse("753c584a-789b-409d-8e9e-89ccac2f0e7d");
    public static Guid temporarily => Guid.Parse("6acec748-2d76-4491-8b77-fd9f990d158a");
    public static Guid definitively => Guid.Parse("bf4c3434-3575-460a-86e2-b757cbdbec07");
    public static Guid AreYouSure => Guid.Parse("db338fe8-8657-492b-9e82-6b44b0bc1252");
    public static Guid EmailIsRequired => Guid.Parse("78e6d2df-ee8d-485f-a7a0-182a33c45988");
    public static Guid FirstNameIsRequired => Guid.Parse("ba415f27-755f-4293-ab4e-71af9fb48983");
    public static Guid LastNameIsRequired => Guid.Parse("1bb7f155-e280-4f99-bb8f-047e451a3409");
    public static Guid DescriptionIsRequired => Guid.Parse("07b78220-ec36-4215-bb32-e2b1b4a0b580");
    public static Guid EmailAlreadyExists => Guid.Parse("28dab09a-c496-40e3-9854-79d7ea96557b");
    public static Guid Required => Guid.Parse("a7cdd2fe-1371-4b44-a996-96e647b834ef");
    public static Guid Avalidemailisrequired => Guid.Parse("d3903b12-c2ff-4dac-9547-851d37f855bc");
    public static Guid Usernamecannotbeempty => Guid.Parse("ce2dfebd-6290-4ce4-b3de-d30aa12e8e3d");
    public static Guid UserNameMustBeAtLeastSixLetters => Guid.Parse("443f280d-3fd0-4746-a605-1b99c574cc84");
    public static Guid UserNameMustNotExceedthrertyLetters => Guid.Parse("17233139-c8b3-462f-a267-75554708b12d");
    public static Guid YourPasswordCannotBeEmpty => Guid.Parse("a43816d6-0f37-4b29-8271-5ca720b094d5");
    public static Guid YourPasswordLengthMustBeAtLeast8Letters => Guid.Parse("0320f0e2-51f3-4c5a-827d-11e3ca1cd0d2");
    public static Guid YourPasswordLengthMustNotExceed16Letters => Guid.Parse("3e4280c5-cfde-48ac-873f-e0592209be5e");
    public static Guid YourPasswordMustContainAtLeastOneUppercaseLetter => Guid.Parse("8ef72372-abdd-4494-bb84-91f25dea99ca");
    public static Guid YourPasswordMustContainAtLeastOneLowercaseLetter => Guid.Parse("942eabc7-9492-4b0a-8ac5-e0dbf5c638b7");
    public static Guid YourPasswordMustContainAtLeastOneNumber => Guid.Parse("67e0837d-0534-4e7e-835b-347bc6d200b9");
    public static Guid YourPasswordMustContainAtLeastOneSymbol => Guid.Parse("1f6c6843-701f-4f9e-b272-5755a68f96ac");
    public static Guid YouHave => Guid.Parse("d7075558-e373-424a-b7ab-8e42fd375352");
    public static Guid AttemptLeft => Guid.Parse("e2b6f38a-0bd2-4e22-bebf-f93219c93784");
    public static Guid TemporarlyLockout => Guid.Parse("82c74c77-fdda-427a-bd40-ffd17ed578e4");
    public static Guid WriteYourPassword => Guid.Parse("a28165f2-0ce0-490d-bfeb-6d572d12f905");
    public static Guid DescritionMissing => Guid.Parse("feeca1b6-3aee-4e53-8079-59f0baeb3e9c");
    public static Guid LoginAsAdmin => Guid.Parse("1688b5b4-5a12-4105-af2f-a35e33484b2f");
    public static Guid NotLoginAsAdmin => Guid.Parse("b5b96c44-b05a-4f52-be62-070b5857789c");
    public static Guid Send => Guid.Parse("71428c8c-935b-424d-92ac-e22d3803c695");
    public static Guid EmailOrPasswordWrong => Guid.Parse("7943411d-e62f-4d79-8cef-712a369652a7");
    public static Guid UserNamelOrPasswordWrong => Guid.Parse("a62a8f65-53d8-41e5-a119-1b096da4cf4a");
    public static Guid ThereIsNothingAtThisAdress => Guid.Parse("536bac48-29c2-4d97-a20d-9e5c85b94933");
    public static Guid LoginError => Guid.Parse("2b07eb20-95cb-4743-b0e6-7eb08490de90");
    public static Guid ErrorOccursWhenLogIn => Guid.Parse("dd4e5e58-e0c9-4923-bcde-162fcb93bf5a");
    public static Guid ErrorOccursPleaseContactSupport => Guid.Parse("667d3f20-2377-49c6-9f95-0eadd44dc4b9");
    public static Guid FileName => Guid.Parse("thtgefhtdjytjhtyrjtdjtd");
    public static Guid AlreadyExists => Guid.Parse("thtgefhtdjytjhtyrjtdjtd");
    public static Guid TheAllowedMaximumSizeIs => Guid.Parse("thtgefhtdjytjhtyrjtdjtd");
    public static Guid FileUpload => Guid.Parse("thtgefhtdjytjhtyrjtdjtd");
    public static Guid Filesize => Guid.Parse("thtgefhtdjytjhtyrjtdjtd");
}
