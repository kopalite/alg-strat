using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;

namespace AlgorithmStrategies.Test
{
    [TestClass]
    public class StrategySettingsTest
    {
        [TestMethod]
        public void Strategy_Settings_Params()
        {
            //Arrange
            var strategy = new SetableStrategy();

            //Act
            strategy.Set(1, 2, 3, TerminationType.TerminateOnFailure);

            //Assert
            Assert.IsTrue(strategy.Type == 1 && strategy.NextId == 2 && strategy.Priority == 3 && strategy.TerminationType == TerminationType.TerminateOnFailure);
        }

        [TestMethod]
        public void Strategy_Settings_FromConfig()
        {
            //Arrange
            var strategy = new SetableStrategy();

            //Act
            strategy.Set(new StrategySettings
            {
                Type = 1,
                NextId = 2,
                Priority = 3,
                TerminationType = TerminationType.TerminateOnFailure
            });

            //Assert
            Assert.IsTrue(strategy.Type == 1 && strategy.NextId == 2 && strategy.Priority == 3 && strategy.TerminationType == TerminationType.TerminateOnFailure);
        }

        [TestMethod]
        public void Strategy_Settings_FluentApi()
        {
            //Act
            var strategies = RuntimeStrategy<UserModel, string>.Create(1, "one", x => true,
                                                                    x => StrategyResult<string>.Failure("r1"))
                                                                .Create(2, "two", x => true,
                                                                    x => StrategyResult<string>.Failure("r1"))
                                                                .Set(new[] 
                                                                {
                                                                    new ConfigSettings
                                                                    {
                                                                        ForId = 1,
                                                                        Type = 100,
                                                                        NextId = 777,
                                                                        Priority = 10000,
                                                                        TerminationType = TerminationType.TerminateOnFailure
                                                                    },
                                                                    new ConfigSettings
                                                                    {
                                                                        ForId = 2,
                                                                        Type = 200,
                                                                        NextId = 888,
                                                                        Priority = 20000,
                                                                        TerminationType = TerminationType.TerminateOnSuccess
                                                                    },
                                                                });

            //Assert
            Assert.IsTrue(strategies.Count() == 2 && strategies.First().NextId == 777 && strategies.Last().NextId == 888);
        }
    }
}
