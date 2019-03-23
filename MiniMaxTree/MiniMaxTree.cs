using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace HassanLib
{
    public class MiniMaxTree
    {
        private IZeroSumGame _data;

        public MiniMaxTree(IZeroSumGame t)
        {
            _data = t;
        }

        public (int alpha, int beta, Action bestAction) Maximise(int searchLimit)
        {
            return Maximise(Int32.MinValue, Int32.MaxValue, searchLimit);
        }
        public (int alpha, int beta, Action bestAction) Minimise(int searchLimit)
        {
            return Minimise(int.MinValue, int.MaxValue, searchLimit);
        }
        public (int alpha, int beta, Action bestAction) Maximise(int alpha, int beta, int searchLimit)
        {
            if (searchLimit <= 0)
                return (_data.StaticEvaluation(), beta, () => { });
            if (!_data.Possible().Any())
                return (_data.StaticEvaluation(), beta, () => { });

            Action best = null;
            foreach ((Action Do, Action Undo) pos in _data.Possible())
            {
                pos.Do.Invoke();
                var reply = Minimise(alpha, beta, searchLimit - 1);
                pos.Undo.Invoke();

                if (reply.beta > alpha)
                {
                    alpha = reply.beta;
                    best = pos.Do;
                }
                if (alpha >= beta) return (alpha, beta, best);
            }
            return (alpha, beta, best);
        }
        public (int alpha, int beta, Action bestAction) Minimise(int alpha, int beta, int searchLimit)
        {
            if (searchLimit <= 0)
                return (alpha, _data.StaticEvaluation(), () => { });
            if (!_data.Possible().Any())
                return (alpha, _data.StaticEvaluation(), () => { });

            Action best = null;
            foreach ((Action Do, Action Undo) pos in _data.Possible())
            {
                pos.Do.Invoke();
                var reply = Maximise(alpha, beta, searchLimit - 1);
                pos.Undo.Invoke();

                if (reply.alpha < beta)
                {
                    beta = reply.alpha;
                    best = pos.Do;
                }
                if (alpha >= beta) return (alpha, beta, best);
            }
            return (alpha, beta, best);
        }
    }
}
