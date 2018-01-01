namespace KristianPiacenti.SimpleStateMachine
{
    public abstract class State<T> where T : class
    {

        //private static State<T> _instance;
        //public static State<T> Instance
        //{
        //    get
        //    {
        //        if (_instance == null)
        //            _instance = new State<T>();
        //        return _instance;
        //    }
        //}

        public abstract void Enter(T owner);
        public abstract void Exit(T owner);
        public abstract void Update(T owner);
       
    }
}