using System.Collections;
using System.Collections.Generic;
using UnityEngine;

public class MouseCursorManager : SingletonMonoBehaviour<MouseCursorManager>
{
   /// <summary>
   /// マウスカーソルの表示と非表示
   /// </summary>
   /// <param name="display"></param>
    public void MouseCursor(bool display)
    {
        //マウスカーソルの表示と中央固定解除
        if (display)
        {
            Cursor.visible = true;
            Cursor.lockState = CursorLockMode.None;
        }
        else//マウスカーソルの非表示と中央固定
        {
            Cursor.visible = false;
            Cursor.lockState = CursorLockMode.Locked;
        }

    }
}
