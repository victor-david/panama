/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Navigation;
using System.Windows.Shapes;
using Restless.App.Panama.Database;
using Restless.App.Panama.ViewModel;

namespace Restless.App.Panama.View
{
    public partial class MainWindow : Window
    {
        #pragma warning disable 1591
        public MainWindow()
        {
            InitializeComponent();
        }
        #pragma warning restore 1591
    }
}