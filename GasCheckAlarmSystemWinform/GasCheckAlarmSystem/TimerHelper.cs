using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;

namespace GasCheckAlarmSystem
{
    public class TimerHelper
    {
        static TimerHelper instance;
        public static TimerHelper GetInstance()
        {
            if (instance == null)
            {
                instance = new TimerHelper();
            }
            return instance;
        }

        List<Timer> timerList = new List<Timer>();

        public void Update(float deltaTime)
        {
            if (timerList.Count > 0)
            {
                for (int i = 0; i < timerList.Count; i++)
                {
                    Timer timer = timerList[i];
                    if (timer != null)
                    {
                        if (!timer.isOver())
                        {
                            timer.Update(deltaTime);
                        }
                        else
                        {
                            timerList.RemoveAt(i);
                        }
                    }
                }
            }
        }

        public void AddTimer(Timer timer)
        {
            timerList.Add(timer);
        }
    }

    public class Timer
    {
        public Action callback_;
        public float delayTime_ = 0;
        float tempTime_ = 0;
        bool isOver_ = false;
        public Timer(float delayTime, Action callback)
        {
            delayTime_ = delayTime;
            callback_ = callback;
        }
        public bool isOver()
        {
            return isOver_;
        }
        public void Update(float deltaTime)
        {
            if (!isOver_)
            {
                tempTime_ += deltaTime;
                if (tempTime_ / 1000 >= delayTime_)
                {
                    isOver_ = true;
                    if (callback_ != null)
                    {
                        callback_();
                    }
                }
            }
        }
    }
}
