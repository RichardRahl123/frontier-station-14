﻿using Content.Shared._NF.Market;
using Robust.Client.AutoGenerated;
using Robust.Client.UserInterface.Controls;
using Robust.Client.UserInterface.XAML;

namespace Content.Client._NF.Market.UI;

[GenerateTypedNameReferences]
public sealed partial class MarketCartProductRow : PanelContainer
{
    public string PrototypeId;
    public MarketCartProductRow(string prototypeId)
    {
        RobustXamlLoader.Load(this);
        PrototypeId = prototypeId;
    }
}
