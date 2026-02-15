//---------------------------------------------------------------------------

#include <vcl.h>
#pragma hdrstop

#include "Unit2.h"
extern int Iteraz;
extern float Precision;
extern bool DelFile;
//---------------------------------------------------------------------------
#pragma package(smart_init)
#pragma resource "*.dfm"
TForm2 *Form2;
//---------------------------------------------------------------------------
__fastcall TForm2::TForm2(TComponent* Owner)
        : TForm(Owner)
{
}
//---------------------------------------------------------------------------

void __fastcall TForm2::BitBtn1Click(TObject *Sender)
{
try {
BitBtn1->Enabled=false;Iteraz=StrToInt(Edit2->Text);
if (StrToFloat(Edit1->Text)<0.0099) {ShowMessage("Точность будет не более 0.01");
Form2->Edit1->Text=FloatToStrF(Precision,ffFixed,3,5);return;};
Precision=StrToFloat(Edit1->Text);
DelFile=CheckBox1->Checked;
}
catch(...){
ShowMessage("Используйте только цифры и запятую.\nУберите лишние пробелы."
"\n Число итераций - только целое.");
Form2->Edit1->Text=FloatToStrF(Precision,ffFixed,3,5);
Form2->Edit2->Text=IntToStr(Iteraz);}
}
//---------------------------------------------------------------------------
void __fastcall TForm2::BitBtn3Click(TObject *Sender)
{
Form2->Close();
}
//---------------------------------------------------------------------------
void __fastcall TForm2::ChangeEdit(TObject *Sender)
{
BitBtn1->Enabled=true;
}
//---------------------------------------------------------------------------
