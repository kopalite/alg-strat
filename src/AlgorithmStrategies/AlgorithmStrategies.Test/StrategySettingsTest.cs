using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using System.Linq;
using System.Diagnostics;

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
            strategy.Set(2, 3, TerminationType.OnFailure);

            //Assert
            Assert.IsTrue(strategy.NextId == 2 && strategy.Priority == 3 && strategy.TerminationType == TerminationType.OnFailure);
        }

        [TestMethod]
        public void Strategy_Settings_FromConfig_Success()
        {
            //Arrange
            var strategy = new SetableStrategy();

            //Act
            strategy.Set(new StrategySettings
            {
                ForId = 10000,
                NextId = 2,
                Priority = 3,
                TerminationType = TerminationType.OnFailure
            });

            //Assert
            Assert.IsTrue(strategy.NextId == 2 && strategy.Priority == 3 && strategy.TerminationType == TerminationType.OnFailure);
        }

        [TestMethod, ExpectedException(typeof(Exception))]
        public void Strategy_Settings_FromConfig_Fail()
        {
            //Arrange
            var strategy = new SetableStrategy();

            //Act
            strategy.Set(new StrategySettings
            {
                ForId = 4444444,
                NextId = 2,
                Priority = 3,
                TerminationType = TerminationType.OnFailure
            });

            //Assert
            Assert.IsTrue(strategy.NextId == 2 && strategy.Priority == 3 && strategy.TerminationType == TerminationType.OnFailure);
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
                                                                    new StrategySettings
                                                                    {
                                                                        ForId = 1,
                                                                        NextId = 777,
                                                                        Priority = 10000,
                                                                        TerminationType = TerminationType.OnFailure
                                                                    },
                                                                    new StrategySettings
                                                                    {
                                                                        ForId = 2,
                                                                        NextId = 888,
                                                                        Priority = 20000,
                                                                        TerminationType = TerminationType.OnSuccess
                                                                    },
                                                                }).ToArray();


            Debugger.NotifyOfCrossThreadDependency();

            //Assert
            Assert.IsTrue(strategies.Count() == 2 && strategies[0].NextId == 888 && strategies[1].NextId == 777);
        }
    }
}
