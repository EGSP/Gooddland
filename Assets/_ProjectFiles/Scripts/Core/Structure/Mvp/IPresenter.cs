﻿using Gasanov.Core.Mvp;
using Gasanov.Core;

namespace Gasanov.Core.Mvp
{
    public interface IPresenter<TView, TModel> : IPresenter where TView : IView
    {
        TView View { get; }
        TModel Model { get; }
    }

    public interface IPresenter : IHandledDisposable
    {
        /// <summary>
        /// Презентер говорит о своем существовании.
        /// </summary>
        void Share();
        
        /// <summary>
        /// Передается ключ для активации из вне.
        /// </summary>
        bool Response(string key);
    }
}