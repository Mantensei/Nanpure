using MantenseiLib;
using MantenseiLib.UI;
using System;
using UnityEngine;

namespace Nanpure.Standard
{    
    public interface IGameStateProvider
    {
        IGameStateEntity GameStateReference { get; }
    }


    public interface IGameStateEntity
    {
        bool MemoMode { get; set; }
        event Action<bool> OnMemoChanged;
    }

    public class GameStateManager : MonoBehaviour, IGameStateProvider, IGameStateEntity
    {
        public event Action<bool> OnMemoChanged;

        bool _memoMode;
        public bool MemoMode
        {
            get => _memoMode;
            set
            {
                var tmp = _memoMode;
                _memoMode = value;

                if (tmp != value) 
                    OnMemoChanged?.Invoke(value);
            }
        }


        public IGameStateEntity GameStateReference => this;
    }
}