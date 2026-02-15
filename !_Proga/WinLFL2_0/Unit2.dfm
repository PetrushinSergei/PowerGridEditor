object Form2: TForm2
  Left = 464
  Top = 304
  HelpContext = 100
  BorderIcons = [biMinimize, biMaximize]
  BorderStyle = bsDialog
  Caption = 'Настройки параметров расчета'
  ClientHeight = 167
  ClientWidth = 315
  Color = clBtnFace
  Font.Charset = DEFAULT_CHARSET
  Font.Color = clWindowText
  Font.Height = -11
  Font.Name = 'MS Sans Serif'
  Font.Style = []
  OldCreateOrder = False
  Position = poMainFormCenter
  PixelsPerInch = 96
  TextHeight = 13
  object Label1: TLabel
    Left = 64
    Top = 16
    Width = 47
    Height = 13
    Caption = 'Точность'
  end
  object Label2: TLabel
    Left = 48
    Top = 64
    Width = 82
    Height = 13
    Caption = 'Число итераций'
  end
  object Label3: TLabel
    Left = 40
    Top = 120
    Width = 257
    Height = 13
    Caption = 'Удаление файлов исходных данных и результатов'
  end
  object Label4: TLabel
    Left = 48
    Top = 136
    Width = 197
    Height = 13
    Caption = 'после завершения работы программы'
  end
  object BitBtn1: TBitBtn
    Left = 208
    Top = 34
    Width = 75
    Height = 25
    Caption = 'Применить'
    Enabled = False
    TabOrder = 0
    OnClick = BitBtn1Click
  end
  object BitBtn3: TBitBtn
    Left = 208
    Top = 82
    Width = 75
    Height = 25
    Caption = 'Закрыть'
    TabOrder = 1
    OnClick = BitBtn3Click
  end
  object Edit1: TEdit
    Left = 56
    Top = 36
    Width = 65
    Height = 21
    TabOrder = 2
    Text = '0.01'
    OnChange = ChangeEdit
  end
  object Edit2: TEdit
    Left = 56
    Top = 84
    Width = 65
    Height = 21
    TabOrder = 3
    Text = '10'
    OnChange = ChangeEdit
  end
  object CheckBox1: TCheckBox
    Left = 16
    Top = 120
    Width = 17
    Height = 33
    TabOrder = 4
    OnClick = ChangeEdit
  end
end
