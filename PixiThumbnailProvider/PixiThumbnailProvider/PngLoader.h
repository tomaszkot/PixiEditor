#pragma once
#include <gdiplus.h>
#include <functional>

class PngLoader
{
    friend class PngImageProvider;

    IStream* CreateStreamOnResource(LPCTSTR fileName, LPCTSTR /*lpType*/)
    {
        IStream* stream;
        if (FileStream::OpenFile(fileName, &stream, false) != S_OK)
        {
            m_logger(std::wstring(L"Unable to open image file: ") + fileName);
            return nullptr;
        }

        m_logger(std::wstring(L"png Image is fine: ") + fileName);
        return stream;
    }

    // Loads a PNG image from the specified stream (using Windows Imaging Component).
    IWICBitmapSource* LoadFromStream(IStream* ipImageStream)
    {
        // initialize return value

        IWICBitmapSource* ipBitmap = NULL;

        // load WIC's PNG decoder

        IWICBitmapDecoder* ipDecoder = NULL;
        if (FAILED(CoCreateInstance(CLSID_WICPngDecoder, NULL, CLSCTX_INPROC_SERVER, __uuidof(ipDecoder), reinterpret_cast<void**>(&ipDecoder))))
            goto Return;

        // load the PNG

        if (FAILED(ipDecoder->Initialize(ipImageStream, WICDecodeMetadataCacheOnLoad)))
            goto ReleaseDecoder;

        // check for the presence of the first frame in the bitmap

        UINT nFrameCount = 0;
        if (FAILED(ipDecoder->GetFrameCount(&nFrameCount)) || nFrameCount != 1)
            goto ReleaseDecoder;

        // load the first frame (i.e., the image)

        IWICBitmapFrameDecode* ipFrame = NULL;
        if (FAILED(ipDecoder->GetFrame(0, &ipFrame)))
            goto ReleaseDecoder;

        // convert the image to 32bpp BGRA format with pre-multiplied alpha

        //   (it may not be stored in that format natively in the PNG resource,

        //   but we need this format to create the DIB to use on-screen)

        WICConvertBitmapSource(GUID_WICPixelFormat32bppPBGRA, ipFrame, &ipBitmap);
        ipFrame->Release();

    ReleaseDecoder:
        ipDecoder->Release();
    Return:
        return ipBitmap;
    }

    //32 - bit DIB from the specified WIC bitmap.
    HBITMAP CreateHBITMAP(IWICBitmapSource* ipBitmap)
    {
        // initialize return value

        HBITMAP hbmp = NULL;

        // get image attributes and check for valid image

        UINT width = 0;
        UINT height = 0;
        if (FAILED(ipBitmap->GetSize(&width, &height)) || width == 0 || height == 0)
            goto Return;

        // prepare structure giving bitmap information (negative height indicates a top-down DIB)

        BITMAPINFO bminfo;
        ZeroMemory(&bminfo, sizeof(bminfo));
        bminfo.bmiHeader.biSize = sizeof(BITMAPINFOHEADER);
        bminfo.bmiHeader.biWidth = width;
        bminfo.bmiHeader.biHeight = -((LONG)height);
        bminfo.bmiHeader.biPlanes = 1;
        bminfo.bmiHeader.biBitCount = 32;
        bminfo.bmiHeader.biCompression = BI_RGB;

        // create a DIB section that can hold the image

        void* pvImageBits = NULL;
        HDC hdcScreen = GetDC(NULL);
        hbmp = CreateDIBSection(hdcScreen, &bminfo, DIB_RGB_COLORS, &pvImageBits, NULL, 0);
        ReleaseDC(NULL, hdcScreen);
        if (hbmp == NULL)
            goto Return;

        // extract the image into the HBITMAP

        const UINT cbStride = width * 4;
        const UINT cbImage = cbStride * height;
        if (FAILED(ipBitmap->CopyPixels(NULL, cbStride, cbImage, static_cast<BYTE*>(pvImageBits))))
        {
            // couldn't extract image; delete HBITMAP

            DeleteObject(hbmp);
            hbmp = NULL;
        }

    Return:
        return hbmp;
    }

    std::function<void(std::wstring)> m_logger;

public:
    PngLoader(std::function<void(std::wstring)> logger)
    {
        m_logger = logger;
    }
};
