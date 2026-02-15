//---------------------------------------------------------------------------

#ifndef MainwlflH
#define MainwlflH
//---------------------------------------------------------------------------
#include <Classes.hpp>
#include <Controls.hpp>
#include <StdCtrls.hpp>
#include <Forms.hpp>
#include <Menus.hpp>
#include <Dialogs.hpp>
#include <ComCtrls.hpp>
#include <ExtCtrls.hpp>
#include <ToolWin.hpp>
#include <ImgList.hpp>
//---------------------------------------------------------------------------
class TForm1 : public TForm
{
__published:	// IDE-managed Components
        TMainMenu *MainMenu1;
        TMenuItem *Afqk1;
        TMenuItem *N1;
        TMenuItem *N2;
        TOpenDialog *OpenDialog1;
        TMenuItem *N3;
        TMenuItem *N4;
        TMenuItem *N5;
        TMenuItem *N6;
        TMenuItem *N7;
        TMenuItem *N8;
        TMenuItem *N9;
        TMenuItem *N10;
        TMenuItem *N11;
        TMenuItem *N12;
        TSaveDialog *SaveDialog1;
        TMenuItem *N13;
        TMenuItem *N14;
        TMenuItem *N15;
        TMenuItem *N16;
        TMenuItem *N17;
        TMenuItem *N18;
        TPopupMenu *PopupMenu1;
        TMenuItem *N19;
        TMenuItem *N20;
        TMenuItem *N21;
        TMenuItem *N23;
        TMenuItem *N22;
        TStatusBar *StatusBar1;
        TPanel *Panel1;
        TPanel *Panel2;
        TPageControl *PageControl1;
        TTabSheet *TabSheet1;
        TMemo *Memo1;
        TTabSheet *TabSheet2;
        TMemo *Memo2;
        TTabSheet *TabSheet3;
        TMemo *Memo3;
        TToolBar *ToolBar1;
        TToolButton *ToolButton1;
        TToolButton *ToolButton2;
        TToolButton *ToolButton3;
        TImageList *ImageList1;
        TMemo *Memo4;
        TMenuItem *N24;
        TMenuItem *N25;
        TMenuItem *N26;
        TMenuItem *MsDos1;
        TTabSheet *TabSheet4;
        TMemo *Memo5;
        TMenuItem *N27;
        TTabSheet *TabSheet5;
        TMemo *Memo6;
        TMenuItem *N28;
        TMenuItem *N29;
        TMenuItem *N30;
        TMenuItem *N31;
        TMenuItem *N32;
        TMenuItem *N33;
        TFontDialog *FontDialog1;
        TMenuItem *N34;
        TSplitter *Splitter1;
        void __fastcall N2Click(TObject *Sender);
        void __fastcall N6Click(TObject *Sender);
        void __fastcall N7Click(TObject *Sender);
        void __fastcall N8Click(TObject *Sender);
        void __fastcall N9Click(TObject *Sender);
        void __fastcall N11Click(TObject *Sender);
        void __fastcall N10Click(TObject *Sender);
        void __fastcall N14Click(TObject *Sender);
        void __fastcall N16Click(TObject *Sender);
        void __fastcall N15Click(TObject *Sender);
        void __fastcall N12Click(TObject *Sender);
        void __fastcall N18Click(TObject *Sender);
        void __fastcall Form1Close(TObject *Sender, TCloseAction &Action);
        void __fastcall Form1Create(TObject *Sender);
        void __fastcall N22Click(TObject *Sender);
        void __fastcall N4Click(TObject *Sender);
        void __fastcall N25Click(TObject *Sender);
        void __fastcall N26Click(TObject *Sender);
        void __fastcall N27Click(TObject *Sender);
        void __fastcall N28Click(TObject *Sender);
        void __fastcall N3Click(TObject *Sender);
        void __fastcall N110Click(TObject *Sender);
        void __fastcall StatBar1(TObject *Sender, WORD &Key,
          TShiftState Shift);
        void __fastcall StatBar2(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
        void __fastcall StatBar3(TObject *Sender);
        void __fastcall StatBar4(TObject *Sender, WORD &Key,
          TShiftState Shift);
        void __fastcall StatBar5(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
        void __fastcall StatBa36(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
        void __fastcall StatBa37(TObject *Sender, WORD &Key,
          TShiftState Shift);
        void __fastcall Stat15(TObject *Sender, WORD &Key,
          TShiftState Shift);
        void __fastcall Stat25(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
        void __fastcall N30Click(TObject *Sender);
        void __fastcall N31Click(TObject *Sender);
        void __fastcall N32Click(TObject *Sender);
        void __fastcall N33Click(TObject *Sender);
        void __fastcall SMemo6(TObject *Sender, WORD &Key,
          TShiftState Shift);
        void __fastcall SbMemo6(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
        void __fastcall M2(TObject *Sender);
        void __fastcall M3(TObject *Sender);
        void __fastcall M5(TObject *Sender);
        void __fastcall M6(TObject *Sender);
        void __fastcall N34Click(TObject *Sender);
        void __fastcall Mem4key(TObject *Sender, WORD &Key,
          TShiftState Shift);
        void __fastcall Mem4Mouse(TObject *Sender, TMouseButton Button,
          TShiftState Shift, int X, int Y);
        
        
private:	// User declarations
//AnsiString PathName;
public:		// User declarations
     __fastcall TForm1(TComponent* Owner);
       
};
//---------------------------------------------------------------------------
extern PACKAGE TForm1 *Form1;
//---------------------------------------------------------------------------
#endif
