using System;
using System.Net;

namespace Tozawa.Auth.Svc.Helper;

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
    public static Guid EmailOrPasswordWrong => Guid.Parse("7943411d-e62f-4d79-8cef-712a369652a7");
    public static Guid UserNamelOrPasswordWrong => Guid.Parse("a62a8f65-53d8-41e5-a119-1b096da4cf4a");
}