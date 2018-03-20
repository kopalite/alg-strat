using System;
using System.Threading;

namespace AlgorithmStrategies
{
    public interface IAlgorithmStrategy<TModel, TResult> : IStrategySettings
    {
        int Id { get; }

        string Name { get; }

        void Set(StrategySettings settings);

        bool IsCandidate(TModel model);
        
        StrategyResult<TResult> Execute(TModel model);

        bool IsTermination(StrategyResult<TResult> result);
    }

    public abstract class StrategyBase<TModel, TResult> : IAlgorithmStrategy<TModel, TResult>
    {
        private StrategySettings _settings;
        private Lazy<StrategySettings> _settingsLazy;

        public abstract int Id { get; }

        public abstract string Name { get; }
        
        public virtual int Type
        {
            get { return _settingsLazy.Value.Type; }
            private set { _settingsLazy.Value.Type = value; }
        }

        public virtual int NextId
        {
            get { return _settingsLazy.Value.NextId; }
            private set { _settingsLazy.Value.NextId = value; }
        }

        public virtual int Priority
        {
            get { return _settingsLazy.Value.Priority; }
            private set { _settingsLazy.Value.Priority = value; }
        }

        public virtual TerminationType TerminationType
        {
            get { return _settingsLazy.Value.TerminationType; }
            private set { _settingsLazy.Value.TerminationType = value; }
        }

        public void Set(StrategySettings settings)
        {
            _settings = settings.Clone();
        }

        public void Set(int type, int nextId, int priority, TerminationType terminationType)
        {

            Type = type;
            NextId = nextId;
            Priority = priority;
            TerminationType = terminationType;
        }

        public StrategyBase()
        {
            _settingsLazy = new Lazy<StrategySettings>(() =>
            {
                if (_settings == null)
                {
                    _settings = new StrategySettings();
                }
                return _settings;
            });
        }

        public virtual bool IsCandidate(TModel model)
        {
            return false;
        }

        public virtual StrategyResult<TResult> Execute(TModel model)
        {
            return StrategyResult<TResult>.Inconclusive();
        }

        public bool IsTermination(StrategyResult<TResult> result)
        {
            return TerminationType == TerminationType.TerminateAlways ||
                   TerminationType == TerminationType.TerminateOnFailure && result.IsFailure ||
                   TerminationType == TerminationType.TerminateOnSuccess && result.IsSuccess;
        }
    }
}
