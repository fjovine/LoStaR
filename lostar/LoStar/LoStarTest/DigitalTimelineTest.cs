using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace LoStarTest
{
    using LoStar;
    using NUnit.Framework;
    using System.IO;
    [TestFixture]
    public class DigitalTimelineTest
    {
        [Test]
        public void DigitalTimeLine_States_AreCorrectlyStored()
        {
            Capture capture = generateCapture();
            DigitalTimeline digitalTimeline = new DigitalTimeline(capture, 0);
            Assert.AreEqual(false, digitalTimeline.InitialState);
            Assert.That(digitalTimeline.Transitions, Is.EqualTo(new[] { 1.0, 1.1, 1.3 }));
            digitalTimeline = new DigitalTimeline(capture, 1);
            Assert.AreEqual(true, digitalTimeline.InitialState);
            Assert.That(digitalTimeline.Transitions, Is.EqualTo(new[] { 1.0, 1.1, 1.2, 1.4 }));
        }

        [Test]
        public void ForEach_ThrowsException_WhenInvalidTimeRange()
        {
            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), 0);
            var ex = Assert.Throws<ArgumentException>(() => digitalTimeLine.ForEach(10.0, 9.0, null));
            StringAssert.Contains("or is more than", ex.Message);
        }

        [Test]
        public void ForEach_Returns0_WhenInitialRangeLaterThanLastTime()
        {
            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), 0);
            bool result = digitalTimeLine.ForEach(3.0, 3.1, null);
            Assert.AreEqual(false, result);
        }

        [Test]
        public void ForEach_Returns0_WhenFinalRangeBeforeFirstTime()
        {
            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), 0);
            bool result = digitalTimeLine.ForEach(0, 0.1, null);
            Assert.AreEqual(true, result);
        }

        [TestCase(0.0, 2.0, 0, true, new bool[] { true, false, true, false, false }, new double[] {0.0, 1, 1.1, 1.3, 2.0})]
        [TestCase(0.0, 1.25, 0, true, new bool[] { true, false, true, false}, new double[] {0, 1, 1.1, 1.25})]
        [TestCase(0.0, 1.3, 0, true, new bool[] { true, false, true, false }, new double[] { 0, 1, 1.1, 1.3 })]
        [TestCase(0.0, 1.3001, 0, true, new bool[] { true, false, true, false,false }, new double[] {0, 1, 1.1, 1.3, 1.3001 })]
        [TestCase(1.05, 1.25, 0, false, new bool[] { false, true, false}, new double[] {1.05, 1.1, 1.25 })]
        [TestCase(1.25, 1.26, 0, true, new bool[] { true , false }, new double[] { 1.25, 1.26 })]
        [TestCase(0.0, 2.0, 1, false, new bool[] { false, true, false, true, false, false }, new double[] {0.0, 1, 1.1, 1.2, 1.4, 2.0 })]
        public void ForEach_ReturnsAllTransitions_ForDifferentRanges(double from, double to, int bit, bool expectedResult, bool[] expectedStates, double[] expectedTimes)
        {
            List<double> transitionTimes = new List<double>();
            List<bool> transitionStates = new List<bool>();

            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), bit);
            bool result = digitalTimeLine.ForEach(from, to,
                (b,t) =>
                {
                    transitionTimes.Add(t);
                    transitionStates.Add(b);
                });
            Assert.AreEqual(expectedResult, result);
            Assert.That(transitionStates, Is.EqualTo(expectedStates));
            Assert.That(transitionTimes, Is.EqualTo(expectedTimes));
        }

        [TestCase(-1, true)]
        [TestCase(0, false)]
        [TestCase(1, true)]
        [TestCase(2, false)]
        [TestCase(3, false)]
        [TestCase(4, false)]
        [TestCase(5, false)]
        [TestCase(6, false)]
        public void StateAt_ReturnsTheCorrectState_WhenTheProperIndexIsGiven(int index, bool expectedState)
        {
            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), 0);
            
            Assert.AreEqual(expectedState, digitalTimeLine.StateAt(index));
        }

        [TestCase(0.0, true)]
        [TestCase(1.0, false)]
        [TestCase(1.099999, false)]
        [TestCase(1.1, true)]
        [TestCase(2, false)]
        public void StateAt_ReturnsTheCorrectState_WhenATimeIsGiven(double time, bool expectedState)
        {
            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), 0);
            
            Assert.AreEqual(expectedState, digitalTimeLine.StateAt(time));
        }

        [TestCase(1.3,  1.1, true)]
        [TestCase(1.3,  null, false)]
        [TestCase(1.05, null, true)]
        [TestCase(1.05, 1.1, false)]
        [TestCase(1.15, 1.1, true)]
        [TestCase(1.15, 1.3, false)]
        [TestCase(1.35, 1.3, true)]
        [TestCase(1.35, null, false)]
        public void GetNearestTransition_ReturnsTheCorrectTransition_WhenATimeIsGiven(double time, double? expectedTime, bool previous)
        {
            DigitalTimeline digitalTimeLine = new DigitalTimeline(generateCapture(), 0);
            digitalTimeLine.Transitions.ForEach((t) =>
            {
                Console.WriteLine("Time " + t);
            });

            Assert.AreEqual(expectedTime, digitalTimeLine.GetNearestTransition(time, previous));
        }

        /// <summary>
        /// This Capture has two bits that have the following states
        /// (s) 1 0
        /// 1.0 H L
        /// 1.1 L H
        /// 1.2 H H
        /// 1.3 H L
        /// 1.4 L L
        /// </summary>
        /// <returns></returns>
        public Capture generateCapture()
        {
            Capture result = new Capture();
            result.TransitionContainer.Add(new Transition() { Time = 1.0, State = 0x02 });
            result.TransitionContainer.Add(new Transition() { Time = 1.1, State = 0x01 });
            result.TransitionContainer.Add(new Transition() { Time = 1.2, State = 0x03 });
            result.TransitionContainer.Add(new Transition() { Time = 1.3, State = 0x02 });
            result.TransitionContainer.Add(new Transition() { Time = 1.4, State = 0x00 });
            return result;
        }
    }
}
