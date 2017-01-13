using System;
using System.Threading.Tasks;
using System.Collections.Generic;
using System.Linq;
using System.Text;

using Android.App;
using Android.Content;
using Android.OS;
using Android.Runtime;
using Android.Views;
using Android.Widget;

namespace CNHSpotlight.WordPress
{
    /// <summary>
    /// Provide extended information about a task.
    /// <para>
    /// Designed to use in cooperation with <see cref="Task{TResult}"/>
    /// </para>
    /// </summary>
    /// <typeparam name="T">Wordpress model needs to be wrapped</typeparam>
    public class ModelWrapper<T>
    {
        public T Data { get; private set; }
        public TaskResult Result { get; private set; }

        /// <summary>
        /// Construct an instance with both data and result
        /// </summary>
        /// <param name="data"></param>
        /// <param name="result"></param>
        public ModelWrapper(T data, TaskResult result)
        {
            Data = data;
            Result = result;
        }

        /// <summary>
        /// Construct an instance with only result (usually used in error situation)
        /// </summary>
        /// <param name="result"></param>
        public ModelWrapper(TaskResult result)
        {
            Result = result;
        }
    }

    public enum TaskResult
    {
        Error = -3,  
        NoInternet = -2,
        NoData = -1,
        Success = 1
    }
}