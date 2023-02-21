#include <shlwapi.h>
#include <thumbcache.h> // For IThumbnailProvider.

#pragma comment(lib, "shlwapi.lib")
#pragma comment(lib, "windowscodecs.lib")
#pragma comment(lib, "Crypt32.lib")
#pragma comment(lib, "msxml6.lib")

#include <fstream>
#include <string>
#include <iostream>

#include <string>
#include <filesystem>
#include "PngImageProvider.h"
namespace fs = std::filesystem;

bool useBmp = false;

static void GetEnvTempPath(std::wstring& input_parameter) {
    wchar_t* env_var_buffer = nullptr;
    std::size_t size = 0;
    if (_wdupenv_s(&env_var_buffer, &size, L"TEMP") == 0 &&
        env_var_buffer != nullptr) {
        input_parameter = static_cast<const wchar_t*>(env_var_buffer);
    }
}

void log(std::wstring line)
{
    try
    {
        std::wstring tmpPath;
        GetEnvTempPath(tmpPath);
        std::wofstream out(tmpPath + L"/PixiEditor/ThumbnailOutput.txt", std::ios_base::app);
        out << line << std::endl;
        out.close();
    }
    catch (const std::exception&)
    {
        //ups log to EventLog?
    }
}

void log(std::string line)
{
    log(std::wstring(line.begin(), line.end()));
}

// this thumbnail provider implements IInitializeWithFile 
class CPixiThumbProvider : public IInitializeWithFile, public IThumbnailProvider
{
public:
    CPixiThumbProvider() : _cRef(1){
    }

    virtual ~CPixiThumbProvider(){
    }

    // IUnknown
    IFACEMETHODIMP QueryInterface(REFIID riid, void **ppv)
    {
        static const QITAB qit[] =
        {
            QITABENT(CPixiThumbProvider, IInitializeWithFile),
            QITABENT(CPixiThumbProvider, IThumbnailProvider),
            { 0 },
        };
        return QISearch(this, qit, riid, ppv);
    }

    IFACEMETHODIMP_(ULONG) AddRef()
    {
        return InterlockedIncrement(&_cRef);
    }

    IFACEMETHODIMP_(ULONG) Release()
    {
        ULONG cRef = InterlockedDecrement(&_cRef);
        if (!cRef)
        {
            delete this;
        }
        return cRef;
    }

    IFACEMETHODIMP Initialize(LPCWSTR pszFilePath, DWORD grfMode);

    // IThumbnailProvider
    IFACEMETHODIMP GetThumbnail(UINT cx, HBITMAP *phbmp, WTS_ALPHATYPE *pdwAlpha);

private:

    long _cRef;
    std::wstring m_filePath;
    std::wstring m_proxyFilePath;
};

HRESULT CPixiThumbProvider_CreateInstance(REFIID riid, void **ppv)
{
    CPixiThumbProvider *pNew = new (std::nothrow) CPixiThumbProvider();
    HRESULT hr = pNew ? S_OK : E_OUTOFMEMORY;
    if (SUCCEEDED(hr))
    {
        hr = pNew->QueryInterface(riid, ppv);
        pNew->Release();
    }
    return hr;
}

IFACEMETHODIMP CPixiThumbProvider::Initialize(LPCWSTR pszFilePath, DWORD /*grfMode*/)
{
    m_filePath = pszFilePath;
    auto proxy = fs::path(m_filePath).stem().string() + (useBmp ? ".bmp" : ".png");
    std::wstring tmpPath;
    GetEnvTempPath(tmpPath);
    m_proxyFilePath = tmpPath + L"/PixiEditor/"+ std::wstring(proxy.begin(), proxy.end());
    log(L"1)  m_filePath:" + std::wstring(m_filePath) + L", m_proxyFilePath: "+ m_proxyFilePath);
    return S_OK;
}

IFACEMETHODIMP CPixiThumbProvider::GetThumbnail(UINT /*cx*/, HBITMAP *phbmp, WTS_ALPHATYPE */*pdwAlpha*/)
{
    log(L"2) GetThumbnail called! m_filePath: "+ std::wstring(m_filePath) + L", useBmp: "+ std::to_wstring(useBmp));
    try
    {
        HBITMAP tmpBmp = nullptr;
        if (useBmp)
            tmpBmp = (HBITMAP)LoadImage(NULL, m_proxyFilePath.c_str(), IMAGE_BITMAP, 0, 0, LR_LOADFROMFILE);
        else
        {
            PngImageProvider pngBitmapProvider([](std::wstring line) { log(line); });
            //tmpBmp = pngBitmapProvider.LoadPngHandle(m_proxyFilePath.c_str());
        }
        if (tmpBmp != nullptr)
        {
            *phbmp = tmpBmp;
            return  S_OK;
        }
        return  S_FALSE;
    }
    catch (const std::exception& ex)
    {
        log(ex.what());
        return  S_FALSE;
    }
}

