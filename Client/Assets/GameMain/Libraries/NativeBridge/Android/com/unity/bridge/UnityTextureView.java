package com.unity.bridge;

import android.content.Context;
import android.graphics.Canvas;
import android.graphics.Rect;
import android.graphics.SurfaceTexture;
import android.util.AttributeSet;
import android.view.Surface;
import android.view.SurfaceHolder;
import android.view.SurfaceView;
import android.view.TextureView;
import android.view.ViewGroup;

import java.lang.reflect.Field;
import java.util.ArrayList;

class UnityTextureView extends TextureView implements TextureView.SurfaceTextureListener {
    private String TAG = "SenseTimeTextureView";

    private SurfaceView surfaceView;

    private Surface surface;

    private SurfaceHolder.Callback mProxyCallback;

    private ProxySurfaceHolder mProxySurfaceHolder = new ProxySurfaceHolder();

    public UnityTextureView(Context context, SurfaceView surfaceView) {
        super(context);
        this.surfaceView = surfaceView;
        init();
    }

    public UnityTextureView(Context context, AttributeSet attrs) {
        super(context, attrs);
        init();
    }

    public UnityTextureView(Context context, AttributeSet attrs, int defStyleAttr) {
        super(context, attrs, defStyleAttr);
        init();
    }

    public UnityTextureView(Context context, AttributeSet attrs, int defStyleAttr, int defStyleRes) {
        super(context, attrs, defStyleAttr, defStyleRes);
        init();
    }

    private void init() {
        setOpaque(false);
        setSurfaceTextureListener(this);
        ViewGroup.LayoutParams layoutParams = this.surfaceView.getLayoutParams();
        if (layoutParams == null)
            layoutParams = new ViewGroup.LayoutParams(0, 0);
        layoutParams.width = 0;
        layoutParams.height = 0;
        this.surfaceView.setLayoutParams(layoutParams);
        try {
            Field field = SurfaceView.class.getDeclaredField("mCallbacks");
            field.setAccessible(true);
            ArrayList<SurfaceHolder.Callback> callbacks = (ArrayList<SurfaceHolder.Callback>)field.get(this.surfaceView);
            this.mProxyCallback = callbacks.get(0);
            synchronized (callbacks) {
                callbacks.clear();
            }
        } catch (Exception e) {
            e.printStackTrace();
        }
    }

    public void onSurfaceTextureAvailable(SurfaceTexture surface, int width, int height) {
        this.surface = new Surface(surface);
        if (this.mProxyCallback != null)
            this.mProxyCallback.surfaceCreated(this.mProxySurfaceHolder);
    }

    public void onSurfaceTextureSizeChanged(SurfaceTexture surface, int width, int height) {
        if (this.mProxyCallback != null)
            this.mProxyCallback.surfaceChanged(this.mProxySurfaceHolder, 0, width, height);
    }

    public boolean onSurfaceTextureDestroyed(SurfaceTexture surface) {
        surface.setOnFrameAvailableListener(null);
        if (this.mProxyCallback != null)
            this.mProxyCallback.surfaceDestroyed(this.mProxySurfaceHolder);
        return false;
    }

    public void onSurfaceTextureUpdated(SurfaceTexture surface) {}

    private class ProxySurfaceHolder implements SurfaceHolder {
        private ProxySurfaceHolder() {}

        public void addCallback(Callback callback) {}

        public void removeCallback(Callback callback) {}

        public boolean isCreating() {
            return false;
        }

        public void setType(int type) {}

        public void setFixedSize(int width, int height) {}

        public void setSizeFromLayout() {}

        public void setFormat(int format) {}

        public void setKeepScreenOn(boolean screenOn) {}

        public Canvas lockCanvas() {
            return null;
        }

        public Canvas lockCanvas(Rect dirty) {
            return null;
        }

        public void unlockCanvasAndPost(Canvas canvas) {}

        public Rect getSurfaceFrame() {
            return null;
        }

        public Surface getSurface() {
            return UnityTextureView.this.surface;
        }
    }
}
