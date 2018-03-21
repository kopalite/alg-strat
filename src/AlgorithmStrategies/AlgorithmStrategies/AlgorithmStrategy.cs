using System;
using System.Threading;

namespace AlgorithmStrategies
{
    public abstract class AlgorithmStrategy<TModel, TResult> : IAlgorithmStrategy<TModel, TResult>
    {
        private IStrategySettings _settings;
        private Lazy<IStrategySettings> _settingsLazy;

        public abstract int Id { get; }

        public abstract string Name { get; }
        
        public virtual bool IsActive
        {
            get; set;
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

        public AlgorithmStrategy()
        {
            _settingsLazy = new Lazy<IStrategySettings>(() =>
            {
                if (_settings == null)
                {
                    _settings = new StrategySettings();
                }
                return _settings;
            });
        }

        public void Set(IStrategySettings settings)
        {
            if (settings == null || settings.ForId != Id)
            {
                throw new Exception("Settings requirements: settings != null && settings.ForId == strategy.Id");
            }
            _settings = settings;
        }

        public void Set(int nextId, int priority, TerminationType terminationType)
        {
            NextId = nextId;
            Priority = priority;
            TerminationType = terminationType;
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
            return !(TerminationType == TerminationType.Never) &&
                   (TerminationType == TerminationType.Always ||
                    TerminationType == TerminationType.OnFailure && result.IsFailure ||
                    TerminationType == TerminationType.OnSuccess && result.IsSuccess);
        }
    }
}
