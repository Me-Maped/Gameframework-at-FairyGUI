package com.unity.bridge;

import android.app.Activity;
import android.content.Context;
import android.content.Intent;
import android.content.res.Configuration;
import android.graphics.Color;
import android.util.AttributeSet;
import android.util.Log;
import android.view.InputEvent;
import android.view.KeyEvent;
import android.view.MotionEvent;
import android.view.SurfaceView;
import android.view.View;
import android.view.ViewGroup;
import android.widget.FrameLayout;

import com.unity3d.player.IUnityPlayerLifecycleEvents;
import com.unity3d.player.UnityPlayer;

import java.lang.reflect.Field;
import java.lang.reflect.Method;

public class UnityFrameLayout extends FrameLayout implements IUnityPlayerLifecycleEvents {
    static final String TAG = "UnityFrameLayout";

    private static SurfaceView surfaceView;

    private static boolean handleTouch = false;

    private static UnityTextureView textureView;

    private static UnityFrameLayout.InternalPlayer mUnityPlayer;

    public UnityFrameLayout(Context context) {
        super(context);
        init((AttributeSet)null, 0);
    }
    public UnityFrameLayout(Context context, AttributeSet attrs) {
        super(context, attrs);
        init(attrs, 0);
    }

    public UnityFrameLayout(Context context, AttributeSet attrs, int defStyle) {
        super(context, attrs, defStyle);
        init(attrs, defStyle);
    }

    public void setVisibility(int visibility) {
        super.setVisibility(visibility);
        for (int i = 0; i < getChildCount(); i++)
            getChildAt(i).setVisibility(visibility);
    }

    public boolean isHandleTouch() {
        return handleTouch;
    }

    public void setHandleTouch(boolean handleTouch) {
        UnityFrameLayout.handleTouch = handleTouch;
    }

    private String updateUnityCommandLineArguments(String cmdLine) {
        return cmdLine;
    }

    private void init(AttributeSet attrs, int defStyle) {
        int flags = 0;
        if (getContext() instanceof Activity)
            flags = (((Activity)getContext()).getWindow().getAttributes()).flags;
        if (mUnityPlayer == null) {
            mUnityPlayer = new UnityFrameLayout.InternalPlayer(getContext(), null);
            nullPersistentUnitySurface();
        }
        mUnityPlayer.setVisibility(0);
        mUnityPlayer.setBackgroundColor(Color.alpha(0));
        if (surfaceView == null && mUnityPlayer.getChildCount() > 0)
            for (int i = 0; i < mUnityPlayer.getChildCount(); i++) {
                if (mUnityPlayer.getChildAt(i) instanceof SurfaceView) {
                    surfaceView = (SurfaceView)mUnityPlayer.getChildAt(i);
                    surfaceView.setZOrderOnTop(false);
                    surfaceView.getHolder().setFormat(-3);
                } else if (mUnityPlayer.getChildAt(i) instanceof UnityTextureView) {
                    textureView = (UnityTextureView)mUnityPlayer.getChildAt(i);
                }
            }
        removeFromParent();
        addView((View)mUnityPlayer, (ViewGroup.LayoutParams)new FrameLayout.LayoutParams(-1, -1));
        if (textureView == null) {
            textureView = new UnityTextureView(getContext(), surfaceView);
            mUnityPlayer.addView((View) textureView, -1, -1);
        }
        if (getContext() instanceof Activity && flags != 0)
            ((Activity)getContext()).getWindow().setFlags(flags, 1024);
        mUnityPlayer.requestFocus();
    }

    private void nullPersistentUnitySurface() {
        Class<UnityPlayer> c = UnityPlayer.class;
        try {
            Field life = c.getDeclaredField("m_PersistentUnitySurface");
            life.setAccessible(true);
            life.set(mUnityPlayer, (Object)null);
        } catch (Exception exception) {}
    }

    private void removeFromParent() {
        ViewGroup viewGroup = (ViewGroup)mUnityPlayer.getParent();
        if (viewGroup != null)
            viewGroup.removeView((View)mUnityPlayer);
    }

    private void addToParent() {
        boolean has = false;
        for (int i = 0; i < getChildCount(); i++) {
            if (getChildAt(i) == mUnityPlayer) {
                has = true;
                break;
            }
        }
        if (!has) {
            removeFromParent();
            addView((View)mUnityPlayer, (ViewGroup.LayoutParams)new FrameLayout.LayoutParams(-1, -1));
        }
    }

    private void avoidKill() {
        if (mUnityPlayer != null) {
            Class<UnityPlayer> c = UnityPlayer.class;
            try {
                Field f = c.getDeclaredField("mProcessKillRequested");
                f.setAccessible(true);
                f.set(mUnityPlayer, Boolean.valueOf(false));
                Field f1 = c.getDeclaredField("mQuitting");
                f1.setAccessible(true);
                f1.set(mUnityPlayer, Boolean.valueOf(false));
            } catch (Exception exception) {}
        }
    }

    private void checkQuitState() {
        if (mUnityPlayer != null) {
            Class<UnityPlayer> c = UnityPlayer.class;
            try {
                Field f = c.getDeclaredField("mState");
                f.setAccessible(true);
                Object o = f.get(mUnityPlayer);
                Method m = o.getClass().getDeclaredMethod("c", new Class[0]);
                m.setAccessible(true);
                m.invoke(o, new Object[] { Boolean.valueOf(false) });
            } catch (Exception exception) {}
        }
    }

    public boolean dispatchTouchEvent(MotionEvent ev) {
        if (handleTouch)
            return super.dispatchTouchEvent(ev);
        return false;
    }

    public void onNewIntent(Intent intent) {
        mUnityPlayer.newIntent(intent);
    }

    public void onDestroy() {
        avoidKill();
        checkQuitState();
        removeAllViews();
    }

    public void onPause() {
        mUnityPlayer.pause();
    }

    public void onResume() {
        addToParent();
        mUnityPlayer.resume();
    }

    public void onLowMemory() {
        mUnityPlayer.lowMemory();
    }

    public void onTrimMemory(int level) {
        if (level == 15)
            mUnityPlayer.lowMemory();
    }

    public void onConfigurationChanged(Configuration newConfig) {
        super.onConfigurationChanged(newConfig);
        mUnityPlayer.configurationChanged(newConfig);
    }

    public void onWindowFocusChanged(boolean hasFocus) {
        super.onWindowFocusChanged(hasFocus);
        mUnityPlayer.windowFocusChanged(hasFocus);
    }

    public boolean dispatchKeyEvent(KeyEvent event) {
        if (event.getAction() == 2)
            return mUnityPlayer.injectEvent((InputEvent)event);
        return super.dispatchKeyEvent(event);
    }

    public boolean onKeyUp(int keyCode, KeyEvent event) {
        return mUnityPlayer.injectEvent((InputEvent)event);
    }

    public boolean onKeyDown(int keyCode, KeyEvent event) {
        return mUnityPlayer.injectEvent((InputEvent)event);
    }

    public boolean onTouchEvent(MotionEvent event) {
        if (handleTouch)
            return mUnityPlayer.injectEvent((InputEvent)event);
        return false;
    }

    public boolean onGenericMotionEvent(MotionEvent event) {
        return mUnityPlayer.injectEvent((InputEvent)event);
    }

    public void onUnityPlayerUnloaded() {
        Log.i("UnityBridgeManager", "unloaded player");
    }

    public void onUnityPlayerQuitted() {
        Log.i("UnityBridgeManager", "quit player");
    }

    private static class InternalPlayer extends UnityPlayer {
        public InternalPlayer(Context context) {
            super(context);
        }

        public InternalPlayer(Context context, IUnityPlayerLifecycleEvents iUnityPlayerLifecycleEvents) {
            super(context, iUnityPlayerLifecycleEvents);
        }

        protected boolean isFinishing() {
            return false;
        }
    }
}
