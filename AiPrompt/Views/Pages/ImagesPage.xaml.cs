// This Source Code Form is subject to the terms of the MIT License.
// If a copy of the MIT was not distributed with this file, You can obtain one at https://opensource.org/licenses/MIT.
// Copyright (C) Leszek Pomianowski and WPF UI Contributors.
// All Rights Reserved.

using AiPrompt.ViewModels.Pages;
using Wpf.Ui.Controls;

namespace AiPrompt.Views.Pages;

public partial class ImagesPage : INavigableView<ImagesViewModel>
{
    public ImagesViewModel ViewModel { get; }

    public ImagesPage(ImagesViewModel viewModel)
    {
        ViewModel = viewModel;
        DataContext = this;
        InitializeComponent();
    }

}
