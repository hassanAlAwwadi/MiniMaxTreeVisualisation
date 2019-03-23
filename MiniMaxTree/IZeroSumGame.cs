using System;
using System.Collections;
using System.Collections.Generic;

namespace HassanLib
{
    public interface IZeroSumGame
    {
        /// <summary>
        /// It returns possible moves that can happen at the current state.
        /// Executing the move returns the undo action that can be used to 
        /// reset the state of the game. 
        /// </summary>
        /// <returns></returns>
        IEnumerable<(Action Move, Action UndoMove)> Possible();

        /// <summary>
        /// gives a value to the current state of the game
        /// </summary>
        /// <returns></returns>
        int StaticEvaluation();
    }
}