using System;
using System.Collections;
using UnityEngine;
using UnityEngine.SceneManagement;

namespace _SPC.Core.Scripts.Managers
{
    public class SceneLoader
    {
        private readonly MonoBehaviour _monoCourotineRunner;

        public SceneLoader(MonoBehaviour monoCourotineRunner)
        {
            _monoCourotineRunner = monoCourotineRunner;
        }
        public void LoadSceneWithCallback(int sceneIndex, Action callback = null)
        {
            _monoCourotineRunner.StartCoroutine(LoadSceneCoroutine(sceneIndex, callback));
        }

        private IEnumerator LoadSceneCoroutine(int sceneIndex, Action callback)
        {
            AsyncOperation asyncLoad = SceneManager.LoadSceneAsync(sceneIndex);
            while (!asyncLoad.isDone)
            {
                yield return null;
            }
            callback?.Invoke();
        }
    }
}