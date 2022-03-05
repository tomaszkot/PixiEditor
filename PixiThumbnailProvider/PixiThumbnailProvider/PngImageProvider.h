#pragma once
#include <shlwapi.h>
#include <wincodec.h>
#include <string>
#include <functional>
#include <gdiplus.h>
#include <stdexcept>
#include "FileStream.h"
#include "Utils.h"
#include "PngLoader.h"

#pragma comment(lib, "gdiplus.lib")
#pragma comment(lib, "windowscodecs.lib")

#include <iostream>
#include <objidl.h>
#include <gdiplus.h>
#pragma warning(disable : 4996)
#pragma warning(disable : 4458)

class PngImageProvider
{
    PngLoader m_loader;
    std::function<void(std::wstring)> m_logger;

public:
    PngImageProvider(std::function<void(std::wstring)> logger) : m_loader(logger)
    {
        m_logger = logger;
    }
        
    HBITMAP PngImageProvider::LoadPngImage(std::wstring filePath)
    {
        Gdiplus::GdiplusStartupInput gdiplusStartupInput;
        ULONG_PTR gdiplusToken;
        Gdiplus::GdiplusStartup(&gdiplusToken, &gdiplusStartupInput, NULL);
        auto bmp = Gdiplus::Bitmap::FromFile(filePath.c_str(), false);
        if (!bmp)
        {
            m_logger(L" Unable to open image file with Gdiplus.");
            return nullptr;
        }
        HBITMAP hBitmap = nullptr;
        bmp->GetHBITMAP(Gdiplus::Color(0xFFFFFFFF), &hBitmap);
        Gdiplus::GdiplusShutdown(gdiplusToken);
        return hBitmap;
    }

    HBITMAP PngImageProvider::LoadImageByStream(std::wstring filePath)
    {
        HBITMAP hbmpSplash = NULL;

        // load the PNG image data into a stream
        IStream* ipImageStream = m_loader.CreateStreamOnResource(filePath.c_str(), L"PNG");
        if (ipImageStream == NULL)
            goto Return;

        // load the bitmap with WIC

        IWICBitmapSource* ipBitmap = m_loader.LoadFromStream(ipImageStream);
        if (ipBitmap == NULL)
            goto ReleaseStream;

        // create a HBITMAP containing the image
        hbmpSplash = m_loader.CreateHBITMAP(ipBitmap);
        ipBitmap->Release();

    ReleaseStream:
        ipImageStream->Release();
    Return:
        return hbmpSplash;
    }

    HBITMAP Load(std::wstring filePath)
    {
        return LoadPngImage(filePath);
    }

};

