using System.Collections;
using System.Collections.Generic;
using System;
using UnityEngine;

namespace Common
{
    /// <summary>
    /// 基底クラス
    /// </summary>
    /// <typeparam name="T">プール対象</typeparam>
    /// <typeparam name="U">コレクション型</typeparam>
    public abstract class ObjectPoolCollectionBase<T, U> where T : MonoBehaviour where U : System.Collections.ICollection, IEnumerable<T>, new()
    {
        public U Pool { get; protected set; } = new U();

        /// <summary>
        /// オブジェクトプールからプール対象のオブジェクトを取得する条件
        /// </summary>
        public delegate bool ConditionToGetFromPool(T instance);

        /// <summary>
        /// オブジェクトプールから取得する際のオプション
        /// </summary>
        /// <param name="instance"></param>
        public delegate void GetPoolOption(T instance);

        /// <summary>
        /// インスタンスの作り方
        /// </summary>
        public delegate T InstantiateMethod();

        /// <summary>
        /// インスタンス生成時に行うオプション
        /// </summary>
        /// <param name="instance"></param>
        /// <returns></returns>
        public delegate T InstantiateOption(T instance);

        /// <summary>
        /// プールから取得する条件
        /// 指定しなければ'!activeSelf'(非活性オブジェクト)
        /// </summary>
        public ConditionToGetFromPool IsGetPool = null;

        /// <summary>
        /// プールから取得する際に呼ばれるメソッド
        /// </summary>
        public GetPoolOption GetPoolMethod = null;

        /// <summary>
        /// インスタンス生成時の作成方法
        /// </summary>
        /// <detail>
        /// * Instantiate
        /// * NGUITools.Add()
        ///  インスタンス化時に細かく条件を指定したい時用。
        ///  上記のように生成メソッドが違う場合や
        ///  生成時にQuaternion.identityに設定する、
        ///  ObjectのParentを指定する時などなど。
        ///  使用側でWrap出来るよう汎用性を持たせる。
        /// </detail>
        public InstantiateMethod CreateInstanceMethod = null;

        /// <summary>
        /// インスタンス生成直後に呼ばれるメソッド
        /// 生成直後のインスタンスに対して行いたい処理を定義。
        /// </summary>
        public InstantiateOption CreateInstanceOption = null;

        /// <summary>
        /// 空コンストラクタ隠蔽
        /// </summary>
        protected ObjectPoolCollectionBase() { }

        /// <summary>
        /// 空コンストラクタ
        /// プール取得条件:!gameObject.activeSelf
        /// 生成方式:Instantiate
        /// 生成時の処理:gameObject.SetActive(true)
        /// </summary>
        public ObjectPoolCollectionBase(GameObject poolObject)
        {
            //取得条件
            IsGetPool = (instance) => { return !instance.gameObject.activeSelf; };
            //取得時に呼ぶ処理
            GetPoolMethod = (instance) => { if (!instance.gameObject.activeSelf) instance.gameObject.SetActive(true); };
            //生成方法
            CreateInstanceMethod = () => { return GameObject.Instantiate(poolObject).GetComponent<T>(); };
            //生成時オプション
            CreateInstanceOption = (instance) =>
            {
                if (!instance.gameObject.activeSelf) instance.gameObject.SetActive(true);
                return instance;
            };
        }

        public ObjectPoolCollectionBase(T poolObject) : this(poolObject.gameObject) { }

        public ObjectPoolCollectionBase(GameObject poolObject, ConditionToGetFromPool isGetPool, GetPoolOption getPoolMethod, InstantiateMethod createInstanceMethod, InstantiateOption createInstanceOption)
        {
            this.IsGetPool = isGetPool;
            this.GetPoolMethod = getPoolMethod;
            this.CreateInstanceMethod = createInstanceMethod;
            this.CreateInstanceOption = createInstanceOption;
        }

        public T Get()
        {
            foreach (var current in Pool)
            {
                if (IsGetPool(current))
                {
                    GetPoolMethod?.Invoke(current);
                    return current;
                }
            }
            var newInstance = Create();
            Add(Pool, newInstance);
            return newInstance;
        }

        private T Create()
        {
            var newInstance = CreateInstanceMethod();
            CreateInstanceOption?.Invoke(newInstance);
            return newInstance;
        }

        protected abstract void Add(U collection, T element);
    }

    /// <summary>
    /// リスト
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolList<T> : ObjectPoolCollectionBase<T, List<T>>
        where T : MonoBehaviour
    {

        public ObjectPoolList(GameObject poolObject) : base(poolObject) { }

        public ObjectPoolList(T target) : base(target) { }

        public ObjectPoolList(GameObject poolObject, ConditionToGetFromPool isGetPool, GetPoolOption getPoolMethod, InstantiateMethod createInstanceMethod, InstantiateOption createInstanceOption)
            : base(poolObject, isGetPool, getPoolMethod, createInstanceMethod, createInstanceOption) { }

        protected override void Add(List<T> collection, T element) { collection.Add(element); }
    }

    /// <summary>
    /// 連結リスト
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolLinkedList<T> : ObjectPoolCollectionBase<T, LinkedList<T>>
        where T : MonoBehaviour
    {

        public ObjectPoolLinkedList(GameObject poolObject) : base(poolObject) { }

        public ObjectPoolLinkedList(T target) : base(target) { }

        public ObjectPoolLinkedList(GameObject poolObject, ConditionToGetFromPool isGetPool, GetPoolOption getPoolMethod, InstantiateMethod createInstanceMethod, InstantiateOption createInstanceOption)
            : base(poolObject, isGetPool, getPoolMethod, createInstanceMethod, createInstanceOption) { }

        protected override void Add(LinkedList<T> collection, T element) { collection.AddLast(element); }
    }

    /// <summary>
    /// キュー
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolQueue<T> : ObjectPoolCollectionBase<T, Queue<T>>
        where T : MonoBehaviour
    {

        public ObjectPoolQueue(GameObject poolObject) : base(poolObject) { }

        public ObjectPoolQueue(T target) : base(target) { }

        public ObjectPoolQueue(GameObject poolObject, ConditionToGetFromPool isGetPool, GetPoolOption getPoolMethod, InstantiateMethod createInstanceMethod, InstantiateOption createInstanceOption)
            : base(poolObject, isGetPool, getPoolMethod, createInstanceMethod, createInstanceOption) { }

        protected override void Add(Queue<T> collection, T element) { collection.Enqueue(element); }
    }

    /// <summary>
    /// スタック
    /// </summary>
    /// <typeparam name="T"></typeparam>
    public class ObjectPoolStack<T> : ObjectPoolCollectionBase<T, Stack<T>>
        where T : MonoBehaviour
    {

        public ObjectPoolStack(GameObject poolObject) : base(poolObject) { }

        public ObjectPoolStack(T target) : base(target) { }

        public ObjectPoolStack(GameObject poolObject, ConditionToGetFromPool isGetPool, GetPoolOption getPoolMethod, InstantiateMethod createInstanceMethod, InstantiateOption createInstanceOption)
            : base(poolObject, isGetPool, getPoolMethod, createInstanceMethod, createInstanceOption) { }

        protected override void Add(Stack<T> collection, T element) { collection.Push(element); }
    }
}