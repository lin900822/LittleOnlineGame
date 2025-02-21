using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace Shared.Common
{
    public class RoutineScheduler
    {
        struct Routine
        {
            public readonly uint RoutineId;
            public readonly long InvokeTimeMs;
            public readonly long RepeatInvokeTimeMs;
            public readonly Action Action;

            public long NextInvokeTimeMs;

            public Routine(uint routineId, long invokeTimeMs, long repeatInvokeTimeMs, Action action)
            {
                RoutineId = routineId;
                InvokeTimeMs = invokeTimeMs;
                RepeatInvokeTimeMs = repeatInvokeTimeMs;
                Action = action;
                NextInvokeTimeMs = 0;
            }
        }

        private uint _nextRoutineId = 0;
        private List<Routine> _routines;
        private HashSet<int> _routineIndexToRemove;

        public static RoutineScheduler Instance
        {
            get
            {
                if (_instance == null)
                {
                    _instance = new RoutineScheduler();
                }

                return _instance;
            }
        }

        private static RoutineScheduler _instance;

        private RoutineScheduler()
        {
            _routines = new List<Routine>();
            _routineIndexToRemove = new HashSet<int>();
        }

        public void Update()
        {
            if (_routines.Count <= 0)
                return;

            _routineIndexToRemove.Clear();

            for (int i = 0; i < _routines.Count; i++)
            {
                var routine = _routines[i];

                if (routine.NextInvokeTimeMs <= TimeUtils.TimeSinceAppStart)
                {
                    routine.Action?.Invoke();
                    if (routine.RepeatInvokeTimeMs > 0)
                    {
                        routine.NextInvokeTimeMs = routine.RepeatInvokeTimeMs + TimeUtils.TimeSinceAppStart;

                        _routines.Add(routine);
                    }

                    _routineIndexToRemove.Add(i);
                }
            }

            foreach (var index in _routineIndexToRemove)
            {
                _routines.RemoveAt(index);
            }
        }

        public uint AddRoutine(Action action, long invokeTimeMs, long repeatInvokeTimeMs = 0)
        {
            var nextId = _nextRoutineId++;
            var routine = new Routine(nextId, invokeTimeMs, repeatInvokeTimeMs, action);
            routine.NextInvokeTimeMs = routine.InvokeTimeMs + TimeUtils.TimeSinceAppStart;

            _routines.Add(routine);

            return nextId;
        }

        public bool RemoveRoutine(uint routineId)
        {
            int index = -1;
            for (int i = 0; i < _routines.Count; i++)
            {
                if (_routines[i].RoutineId == routineId)
                    index = i;
            }

            if (index >= 0)
            {
                _routines.RemoveAt(index);
                return true;
            }

            return false;
        }
    }
}