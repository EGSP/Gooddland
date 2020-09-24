using System;

namespace Gasanov.Utils.Updaters
{
    // Здесь создаются и action, и function
    public static class UpdaterFactory
    {
        public static FunctionUpdater CreateFunction(Func<float,bool> function,string name ="default",
            UpdateType updateType = UpdateType.Update)
        {
            var fu = new FunctionUpdater(function, name);

            var hook = Hook.Instance;
            switch (updateType)
            {
                case UpdateType.Update:
                    hook.OnUpdate += fu.Update;
                    break;
                case UpdateType.Fixed:
                    hook.OnFixedUpdate += fu.Update;
                    break;
                case UpdateType.Late:
                    hook.OnLateUpdate += fu.Update;
                    break;
            }

            return fu;
        }

        public static void Destroy(IUpdater updater)
        {
            var hook = Hook.Instance;
            
            hook.OnUpdate -= updater.Update;
            hook.OnFixedUpdate -= updater.Update;
            hook.OnLateUpdate -= updater.Update;
        }
    }

    public enum UpdateType
    {
        Update,
        Fixed,
        Late
    }
}