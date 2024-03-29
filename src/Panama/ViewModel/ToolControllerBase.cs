/*
 * Copyright 2019 Victor D. Sandiego
 * This file is part of Panama.
 * Panama is free software: you can redistribute it and/or modify it under the terms of the GNU General Public License v3.0
 * Panama is distributed in the hope that it will be useful, but without warranty of any kind.
*/
using Restless.Panama.Tools;
using Restless.Toolkit.Mvvm;
using System;
using System.Collections.ObjectModel;

namespace Restless.Panama.ViewModel
{
    /// <summary>
    /// Represents the base class for a tool controller. This class must be inherited.
    /// </summary>
    /// <typeparam name="VM">The view model uses this controller and becomes its owner.</typeparam>
    public abstract class ToolControllerBase<VM> : ObservableObject  where VM : ApplicationViewModel
    {
        #region Private
        private string updatedHeader;
        private string notFoundHeader;
        #endregion
        
        /************************************************************************/

        #region Public properties
        /// <summary>
        /// When implemented in a derived class, gets the controller-specific task id.
        /// Must be unique among controllers.
        /// </summary>
        public abstract int TaskId
        {
            get;
        }

        /// <summary>
        /// Gets the scanner object associated with this tool
        /// </summary>
        public abstract Scanner Scanner
        {
            get;
        }

        /// <summary>
        /// Gets the collection of <see cref="FileScanResult"/> objects that represent updated items.
        /// </summary>
        public ObservableCollection<FileScanResult> Updated
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets the collection of <see cref="FileScanResult"/> objects that represent not found items.
        /// </summary>
        public ObservableCollection<FileScanResult> NotFound
        {
            get;
            private set;
        }

        /// <summary>
        /// Gets a header string value for items that have nee updated.
        /// </summary>
        public string UpdatedHeader
        {
            get { return updatedHeader; }
            protected set
            {
                SetProperty(ref updatedHeader, value);
            }
        }


        /// <summary>
        /// Gets a header string value for items that are considered not found.
        /// </summary>
        public string NotFoundHeader
        {
            get { return notFoundHeader; }
            protected set
            {
                SetProperty(ref notFoundHeader, value);
            }
        }
        #endregion

        /************************************************************************/

        #region Protected properties
        /// <summary>
        /// Gets the view model that owns this controller
        /// </summary>
        protected VM Owner
        {
            get;
            private set;
        }

        #endregion

        /************************************************************************/

        #region Constructor
        /// <summary>
        /// Initializes a new instance of the <see cref="ToolControllerBase{VM}"/> class.
        /// </summary>
        /// <param name="owner">The view model that owns this controller.</param>
        public ToolControllerBase(VM owner)
        {
            Owner = owner ?? throw new ArgumentNullException(nameof(owner));
            Updated = new ObservableCollection<FileScanResult>();
            NotFound = new ObservableCollection<FileScanResult>();
        }
        #endregion

        /************************************************************************/

        #region Public methods
        /// <summary>
        /// When implemented in a derived class, runs the operation specific to its controller.
        /// </summary>
        public abstract void Run();

        /// <summary>
        /// Use this method from a derived class to clear the observable collections, in preparation to starting the operation.
        /// </summary>
        protected void ClearCollections()
        {
            Updated.Clear();
            NotFound.Clear();
        }

        /// <summary>
        /// Adds the specified item to the Updated collection.
        /// </summary>
        /// <param name="item">The item to add</param>
        protected void AddToUpdated(FileScanResult item)
        {
            Updated.Add(item);
        }

        /// <summary>
        /// Adds the specified item to the NotFound collection.
        /// </summary>
        /// <param name="item">The item to add</param>
        protected void AddToNotFound(FileScanResult item)
        {
            NotFound.Add(item);
        }


        /// <summary>
        /// Removes the specified item to the NotFound collection.
        /// </summary>
        /// <param name="item">The item to remove.</param>
        protected void RemoveFromNotFound(FileScanResult item)
        {
            NotFound.Remove(item);
        }
        #endregion
    }
}