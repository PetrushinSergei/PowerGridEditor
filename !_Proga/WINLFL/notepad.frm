VERSION 5.00
Begin VB.Form frmNotePad 
   Caption         =   "WinLFL"
   ClientHeight    =   6420
   ClientLeft      =   1005
   ClientTop       =   1320
   ClientWidth     =   10050
   BeginProperty Font 
      Name            =   "MS Sans Serif"
      Size            =   8.25
      Charset         =   204
      Weight          =   700
      Underline       =   0   'False
      Italic          =   0   'False
      Strikethrough   =   0   'False
   EndProperty
   LinkTopic       =   "Form1"
   MDIChild        =   -1  'True
   ScaleHeight     =   6420
   ScaleMode       =   0  'User
   ScaleWidth      =   179.146
   Begin VB.TextBox Text1 
      Height          =   6255
      HideSelection   =   0   'False
      Left            =   0
      MultiLine       =   -1  'True
      ScrollBars      =   2  'Vertical
      TabIndex        =   0
      Top             =   0
      Width           =   9855
   End
   Begin VB.Menu mnuFile 
      Caption         =   "&File"
      Begin VB.Menu mnuFileNew 
         Caption         =   "&New"
      End
      Begin VB.Menu mnuFileOpen 
         Caption         =   "&Open..."
      End
      Begin VB.Menu mnuFileClose 
         Caption         =   "&Close"
      End
      Begin VB.Menu mnuFileSave 
         Caption         =   "&Save"
      End
      Begin VB.Menu mnuFileSaveAs 
         Caption         =   "Save &As..."
      End
      Begin VB.Menu mnuFSep 
         Caption         =   "-"
      End
      Begin VB.Menu mnuFileExit 
         Caption         =   "E&xit"
      End
      Begin VB.Menu mnuRecentFile 
         Caption         =   "-"
         Index           =   0
         Visible         =   0   'False
      End
      Begin VB.Menu mnuRecentFile 
         Caption         =   "RecentFile1"
         Index           =   1
         Visible         =   0   'False
      End
      Begin VB.Menu mnuRecentFile 
         Caption         =   "RecentFile2"
         Index           =   2
         Visible         =   0   'False
      End
      Begin VB.Menu mnuRecentFile 
         Caption         =   "RecentFile3"
         Index           =   3
         Visible         =   0   'False
      End
      Begin VB.Menu mnuRecentFile 
         Caption         =   "RecentFile4"
         Index           =   4
         Visible         =   0   'False
      End
      Begin VB.Menu mnuRecentFile 
         Caption         =   "RecentFile5"
         Index           =   5
         Visible         =   0   'False
      End
   End
   Begin VB.Menu mnuEdit 
      Caption         =   "&Edit"
      Begin VB.Menu mnuEditCut 
         Caption         =   "Cu&t"
         Shortcut        =   ^X
      End
      Begin VB.Menu mnuEditCopy 
         Caption         =   "&Copy"
         Shortcut        =   ^C
      End
      Begin VB.Menu mnuEditPaste 
         Caption         =   "&Paste"
         Shortcut        =   ^V
      End
      Begin VB.Menu mnuEditDelete 
         Caption         =   "De&lete"
         Shortcut        =   {DEL}
      End
      Begin VB.Menu mnuESep1 
         Caption         =   "-"
      End
      Begin VB.Menu mnuEditSelectAll 
         Caption         =   "Select &All"
      End
      Begin VB.Menu mnuEditTime 
         Caption         =   "Time/&Date"
      End
   End
   Begin VB.Menu mnuSearch 
      Caption         =   "&Search"
      Begin VB.Menu mnuSearchFind 
         Caption         =   "&Find"
      End
      Begin VB.Menu mnuSearchFindNext 
         Caption         =   "Find &Next"
         Shortcut        =   {F3}
      End
   End
   Begin VB.Menu mnuOptions 
      Caption         =   "&Options"
      Begin VB.Menu mnuOptionsToolbar 
         Caption         =   "&Toolbar"
      End
      Begin VB.Menu mnuFont 
         Caption         =   "&Font"
         Begin VB.Menu mnuFontName 
            Caption         =   "FontName"
            Index           =   0
         End
      End
   End
   Begin VB.Menu mnuWindow 
      Caption         =   "&Window"
      WindowList      =   -1  'True
      Begin VB.Menu mnuWindowCascade 
         Caption         =   "&Cascade"
      End
      Begin VB.Menu mnuWindowTile 
         Caption         =   "&Tile"
      End
      Begin VB.Menu mnuWindowArrange 
         Caption         =   "&Arrange Icons"
      End
   End
   Begin VB.Menu mnuRun 
      Caption         =   "&Run"
      Begin VB.Menu mnuAC 
         Caption         =   "&AC Model"
      End
   End
End
Attribute VB_Name = "frmNotePad"
Attribute VB_GlobalNameSpace = False
Attribute VB_Creatable = False
Attribute VB_PredeclaredId = True
Attribute VB_Exposed = False
'*** Child form for the MDI Notepad sample application  ***
'**********************************************************
Option Explicit
Const N1 = 300 'Максимальное число узлов
Const M1 = 450 'Максимальное число ветвей

Dim NSB, N, NO, M, NN(N1), NM(2, M1), NK(N1), NL(N1)
Dim VN1(N1), VN2(N1), DN(N1) 'Напряжение  и фаза
Dim MET
Dim VA(N1), VR(N1), VSBA, VSBR 'Действительная и мнимая составляющие напряжений
Dim GR(M1), BX(M1), AK(M1), RK(M1) 'Проводимости и К тр
Dim PS(N1), PP(N1), QS(N1), QMIN(N1), QMAX(N1)
Dim GY(M1), BY(M1), BB(M1), GG(M1), PMAX(M1) 'проводимости ветвей
Dim R(50) 'рабочий массив
Dim YY(2 * N1), ss(2 * N1)
Dim D(2 * N1, 1), RR(2 * N1), DS1(2 * N1), DS(2 * N1)
Dim keyn(10)              'Ключи печати
Dim eps, iter          'Точность,  итерация
Dim FILE_ID As String
Dim FILE_OUT As String
Dim STR1$
Dim i, j  'Переменные циклов

Private Sub Form_Load()
    Dim i As Integer        ' Counter variable.
    
    ' Assign the name of the first font to a font
    ' menu entry, then loop through the fonts
    ' collection, adding them to the menu
    mnuFontName(0).Caption = Screen.Fonts(0)
    For i = 1 To Screen.FontCount - 1
        Load mnuFontName(i)
        mnuFontName(0).Caption = Screen.Fonts(i)
    Next
End Sub

Private Sub Form_QueryUnload(Cancel As Integer, UnloadMode As Integer)
    Dim strMsg As String
    Dim strFilename As String
    Dim intResponse As Integer

    ' Check to see if the text has been changed.
    If FState(Me.Tag).Dirty Then
        strFilename = Me.Caption
        strMsg = "The text in [" & strFilename & "] has changed."
        strMsg = strMsg & vbCrLf
        strMsg = strMsg & "Do you want to save the changes?"
        intResponse = MsgBox(strMsg, 51, frmMDI.Caption)
        Select Case intResponse
            Case 6      ' User chose Yes.
                If Left(Me.Caption, 8) = "Untitled" Then
                    ' The file hasn't been saved yet.
                    strFilename = "untitled.txt"
                    ' Get the strFilename, and then call the save procedure, GetstrFilename.
                    strFilename = GetFileName(strFilename)
                Else
                    ' The form's Caption contains the name of the open file.
                    strFilename = Me.Caption
                End If
                ' Call the save procedure. If strFilename = Empty, then
                ' the user chose Cancel in the Save As dialog box; otherwise,
                ' save the file.
                If strFilename <> "" Then
                    SaveFileAs strFilename
                End If
            Case 7      ' User chose No. Unload the file.
                Cancel = False
            Case 2      ' User chose Cancel. Cancel the unload.
                Cancel = True
        End Select
    End If
End Sub

Private Sub Form_Resize()
    ' Expand text box to fill the current child form's internal area.
    Text1.Height = ScaleHeight
    Text1.Width = ScaleWidth
End Sub

Private Sub Form_Unload(Cancel As Integer)
    ' Show the current form instance as deleted
    FState(Me.Tag).Deleted = True
    
    ' Hide the toolbar edit buttons if no notepad windows exist.
    If Not AnyPadsLeft() Then
        frmMDI.imgCutButton.Visible = False
        frmMDI.imgCopyButton.Visible = False
        frmMDI.imgPasteButton.Visible = False
        ' Toggle the public tool state variable
        gToolsHidden = True
        ' Call the recent file list procedure
        GetRecentFiles
    End If
End Sub



Private Sub mnuAC_Click()
' Запуск программы расчета устан. режима
     Dim strFilename, File_EXT As String
     Dim LFLflag As Boolean ' Флаги
          LFLflag = False
         If Left(Me.Caption, 8) = "Untitled" Then
        ' The file hasn't been saved yet.
        ' Get the filename, and then call the save procedure, GetFileName.
        strFilename = GetFileName(strFilename)
        LFLflag = False
    Else
        ' The form's Caption contains the name of the open file.
        strFilename = Me.Caption
        LFLflag = True
    End If
         FILE_ID = strFilename
         If LFLflag = False Then GoTo ERROR1
      'Unload Me
    ' FileNew
    '      Screen.ActiveControl = "Ну вот и начали" + Chr$(13) + Chr$(10)
   ' Проверочка
      On Error GoTo ERROR2
   Open FILE_ID For Input As #1
     Close
   File_EXT = Right$(FILE_ID, 3)
   On Error GoTo ERROR3
   'If File_EXT = "shm" Then
   SHM
   'Screen.ActiveControl = Screen.ActiveControl + "Открыли и прочитали " + File_EXT + Chr$(13) + Chr$(10)
   OUTDD
'   GRAF1  ' Сервис схемы, поиск ошибок
    YM
   OUTDD
   
  ' Вот тут и запускается расчет режима!!!
 '  Newton
   '  Screen.ActiveControl = Screen.ActiveControl + "Расчет закончен" + Chr$(13) + Chr$(10)
   
     GoTo EXT
ERROR1:
 MsgBox "Нет данных для расчета", , "Ошибка": GoTo EXT
ERROR2:
 MsgBox "Ошибка открытия файла данных для расчета", , "Ошибка": GoTo EXT
ERROR3:
 MsgBox "Ошибка открытия файла " + FILE_ID, , "Ошибка": GoTo EXT
EXT:
  Close
   End Sub

Private Sub mnuEditCopy_Click()
    ' Call the copy procedure
    EditCopyProc
End Sub

Private Sub mnuEditCut_Click()
    ' Call the cut procedure
    EditCutProc
End Sub

Private Sub mnuEditDelete_Click()
    ' If the mouse pointer is not at the end of the notepad...
    If Screen.ActiveControl.SelStart <> Len(Screen.ActiveControl.Text) Then
        ' If nothing is selected, extend the selection by one.
        If Screen.ActiveControl.SelLength = 0 Then
            Screen.ActiveControl.SelLength = 1
            ' If the mouse pointer is on a blank line, extend the selection by two.
            If Asc(Screen.ActiveControl.SelText) = 13 Then
                Screen.ActiveControl.SelLength = 2
            End If
        End If
        ' Delete the selected text.
        Screen.ActiveControl.SelText = ""
    End If
End Sub

Private Sub mnuEditPaste_Click()
    ' Call the paste procedure.
    EditPasteProc
End Sub

Private Sub mnuEditSelectAll_Click()
    ' Use SelStart & SelLength to select the text.
    frmMDI.ActiveForm.Text1.SelStart = 0
    frmMDI.ActiveForm.Text1.SelLength = Len(frmMDI.ActiveForm.Text1.Text)
End Sub

Private Sub mnuEditTime_Click()
    ' Insert the current time and date.
    Text1.SelText = Now
End Sub

Private Sub mnuFileClose_Click()
    ' Unload this form.
    Unload Me
End Sub

Private Sub mnuFileExit_Click()
    ' Unloading the MDI form invokes the QueryUnload event
    ' for each child form, and then the MDI form.
    ' Setting the Cancel argument to True in any of the
    ' QueryUnload events cancels the unload.
    Unload frmMDI
End Sub

Private Sub mnuFileNew_Click()
    ' Call the new form procedure
    FileNew
End Sub

Private Sub mnuFontName_Click(index As Integer)
    ' Assign the selected font to the textbox fontname property.
    Text1.FontName = mnuFontName(index).Caption
End Sub

Private Sub mnuFileOpen_Click()
    ' Call the file open procedure.
    FileOpenProc
End Sub

Private Sub mnuFileSave_Click()
    Dim strFilename As String

    If Left(Me.Caption, 8) = "Untitled" Then
        ' The file hasn't been saved yet.
        ' Get the filename, and then call the save procedure, GetFileName.
        strFilename = GetFileName(strFilename)
    Else
        ' The form's Caption contains the name of the open file.
        strFilename = Me.Caption
    End If
    ' Call the save procedure. If Filename = Empty, then
    ' the user chose Cancel in the Save As dialog box; otherwise,
    ' save the file.
    If strFilename <> "" Then
        SaveFileAs strFilename
    End If
End Sub

Private Sub mnuFileSaveAs_Click()
    Dim strSaveFileName As String
    Dim strDefaultName As String
    
    ' Assign the form caption to the variable.
    strDefaultName = Me.Caption
    If Left(Me.Caption, 8) = "Untitled" Then
        ' The file hasn't been saved yet.
        ' Get the filename, and then call the save procedure, strSaveFileName.
        
        strSaveFileName = GetFileName("Untitled.txt")
        If strSaveFileName <> "" Then SaveFileAs (strSaveFileName)
        ' Update the list of recently opened files in the File menu control array.
        UpdateFileMenu (strSaveFileName)
    Else
        ' The form's Caption contains the name of the open file.
        
        strSaveFileName = GetFileName(strDefaultName)
        If strSaveFileName <> "" Then SaveFileAs (strSaveFileName)
        ' Update the list of recently opened files in the File menu control array.
        UpdateFileMenu (strSaveFileName)
    End If

End Sub

Private Sub mnuOptions_Click()
    ' Toggle the Checked property to match the .Visible property.
    mnuOptionsToolbar.Checked = frmMDI.picToolbar.Visible
End Sub

Private Sub mnuOptionsToolbar_Click()
    ' Call the toolbar procedure, passing a reference
    ' to this form instance.
    OptionsToolbarProc Me
End Sub

Private Sub mnuRecentFile_Click(index As Integer)
    ' Call the file open procedure, passing a
    ' reference to the selected file name
    OpenFile (mnuRecentFile(index).Caption)
    ' Update the list of recently opened files in the File menu control array.
    GetRecentFiles
End Sub

Private Sub mnuSearchFind_Click()
    ' If there is text in the textbox, assign it to
    ' the textbox on the Find form, otherwise assign
    ' the last findtext value.
    If Me.Text1.SelText <> "" Then
        frmFind.Text1.Text = Me.Text1.SelText
    Else
        frmFind.Text1.Text = gFindString
    End If
    ' Set the public variable to start at the beginning.
    gFirstTime = True
    ' Set the case checkbox to match the public variable
    If (gFindCase) Then
        frmFind.chkCase = 1
    End If
    ' Display the Find form.
    frmFind.Show vbModal
End Sub

Private Sub mnuSearchFindNext_Click()
    ' If the public variable isn't empty, call the
    ' find procedure, otherwise call the find menu
    If Len(gFindString) > 0 Then
        FindIt
    Else
        mnuSearchFind_Click
    End If
End Sub

Private Sub mnuWindowArrange_Click()
    ' Arrange the icons for any minimzied child forms.
    frmMDI.Arrange vbArrangeIcons
End Sub

Private Sub mnuWindowCascade_Click()
    ' Cascade the child forms.
    frmMDI.Arrange vbCascade
End Sub

Private Sub mnuWindowTile_Click()
    ' Tile the child forms.
    frmMDI.Arrange vbTileHorizontal
End Sub

Private Sub Text1_Change()
    ' Set the public variable to show that text has changed.
    FState(Me.Tag).Dirty = True
End Sub



'********************************************************
' Далее - текст программы расчета установившегося режима
' *************** Исходные данные***********************************
' Исходные  данные считываются  из  файла,  имя которого  задается в
' первой строке програмы.
' Структура файла (на примере файла demo1.dat)
'

'
'   Следующие строки - данные по узлам
'101  1   110     0    46.188   23.094    0.00   0
'102  5   115     0     0.00     0.00     0.00   0
' |   |    |      |      |        |        |     |
' N   |    |    Фаза   Pузла   Qузла    Gшунта   Bшунта
'узла |    |_Напряжение
'     |_Тип узла
'
' Напряжение  узла задается  в кВ,  мощности в  мВт и  мВАр, фаза  в
' градусах, проводимости шунтов - в микроСименсах. Тип узла:
' 1 - PQ; 2 - PU; 5 -балансирующий.
' БАЛАНСИРУЮЩИЙ УЗЕЛ ДОЛЖЕН БЫТЬ ПОСЛЕДНИЙ!
'
'    Следующие строки - данные по ветвям.
'101 102    10.00    20.00     1.00     0.00   0.00
' номера      R       X         Kтр      G       B
' узлов
'
' Для трансформаторных узлов на первом месте указывается узел с
' высоким напряжением и Kтр задается >1
' Этот пpимеp описан в [2]  стр.152, решение приведено
' на стр.217-219
'
' ******************** Переменные****************************
' NAM  -  номер  (имя)  узла
' Pн, Qн - соответственно активная и реактивная мощности потребления
' Pг, Qг - активная и реактивная мощности генерации узла.
' P  и  Q -расчетная нагрузка
' U  -  номинальное  напряжение  узла  или  начальное приближение
' D  -  угол  вектора  напряжения.
' Ntyp - тип  узла
'    Тип узла может быть следующим:
'    PQ - узел, в котором фиксированы P и Q
'    PU - узел, в котором фиксированы P и U
'    UD - балансирующий  узел
'    Данные для каждой ветви сети:
' NM1,  NM2 - имена узлов, связывающих ветвь
' R, X - соответственно активное и реактивное сопротивление ветви;
'        в ветвях, являющихся трансформаторами, R и X трансформаторов
'        следует привести к высокой  стороне U
' bb, gg - соответственно активная и реактивная проводимости на землю
'     по  П-образной  схеме  замещения, задается в микроСименсах
' aK, rK - действительная и мнимая составляющие К тр;
' aKij задается > 1, где
'       i - узел  с высоким U, j - узел  с низким U
'     Общая информация о схеме и параметр точности
' N, M - число узлов и ветвей схемы.
' eps  - точность расчета.
'Gm - взаимная активная
'Bm - и реактивная проводимости
'G , B - собственные проводимости
'
'*********** Начало Программы *********************

 Private Sub SHM()
'ВВод данных по нашим форматам
' Dim strFilename As String
 
 Dim RP1$, RP2$, RP3$, LF$
 Dim NO 'Число узлов
 LF$ = Chr$(13) + Chr$(10)
   ' strFilename = GetFileName(strFilename)
 On Error GoTo Err2
 Open FILE_ID For Input As #1
 If EOF(1) Then GoTo Err1
 Line Input #1, RP1$ ' Читаем первую пустую строку.
 Input #1, NO, M, eps
 N = NO - 1
 For i = 1 To 10
 Input #1, keyn(i)
 Next
 
 
' *********************** Отладочная печать
'RP1$ = N
' RP2$ = M
' RP3$ = eps
 'Screen.ActiveControl = Screen.ActiveControl + "Число узлов " + RP1$ + " Число ветвей " + RP2$ + "Точность " + RP3$ + LF$
 ' ***************************
 'Читаем остальные узлы
 For i = 1 To NO
  Input #1, NN(i), NK(i), VN1(i), DN(i), PS(i), QS(i), GG(i), BB(i)
    'Определяем  остальные величины
  'Признак базисного узла
  If NK(i) = 6 Or NK(i) = 5 Then
  NK(i) = 6
  NSB = NN(i)
  VSBA = VN1(i)
  VN2(i) = VN1(i)
  VSBR = 0
  PP(i) = 0
  QMIN(i) = 0
  QMAX(i) = 0
 End If
' Дополняем данные по остальным узлам
  PP(i) = 0
  QMIN(i) = 0
  QMAX(i) = 0
 Next
'Ветви
For i = 1 To M
Input #1, NM(1, i), NM(2, i), GR(i), BX(i), AK(i), BY(i), GY(i)
 BY(i) = -BY(i)
 RK(i) = 0
Next
GoTo EXT
Err2:
MsgBox "Ошибка открытия файла данных *.shm", , "Ошибка 102"
Err1:
MsgBox "Ошибка данных файла формата *.shm", , "Ошибка 101"
EXT:
Close #1
End Sub
 
Private Sub OUTDD()
 Dim LF$, RP1$, RP2$, RP3$ 'Перевод строки и рабочие массивы печати
 Dim RP4$, RP5$, RP6$, RP7$, RP8$, RP9$, RP10$, RP11$, RP12$
 Dim K
 LF$ = Chr$(13) + Chr$(10)
 Screen.ActiveControl = Screen.ActiveControl + LF$ + "        Параметры исходной схемы " + LF$
 
 RP1$ = N
 RP2$ = M
 RP3$ = eps
 Screen.ActiveControl = Screen.ActiveControl + "Число узлов " + RP1$ + " Число ветвей " + RP2$ + "Точность " + RP3$ + LF$
 Screen.ActiveControl = Screen.ActiveControl + "Информация об узлах" + LF$
 Screen.ActiveControl = Screen.ActiveControl + "N узла     " + "Umin          " + "Umax          " + "Фаза        " + _
        "Pн      " + "Qн     " + "Pг      " + "Qг     " + "Qг min   " + "Qг max   " + "G      " + "B" + LF$
 For i = 1 To N
 RP1$ = Format(NN(i), "###### ")
 RP2$ = Format(VN1(i), "####.0 ")
 RP3$ = Format(VN2(i), "####.0 ")
 RP4$ = Format(DN(i), "##.0 ")
 RP5$ = Format(PP(i), "###0.0 ")
 RP6$ = Format(QMIN(i), "####.0 ")
 RP7$ = Format(QMAX(i), "###0.0 ")
 RP8$ = Format(PS(i), "####.0 ")
 RP9$ = Format(QS(i), "####.0 ")
 RP10$ = Format(GG(i), "####.0 ")
 RP11$ = Format(BB(i), "####.0 ")
 'Сервис узлов
 K = 0
 RP12$ = ""
 If (PS(i) <= 0 And QS(i) <= 0) Then GoTo 53
 If (PS(i) >= QS(i)) Then GoTo 51
 RP12$ = " P<Q ?"
 K = 1
51: QS(i) = QS(i) * 4
If (PS(i) <= QS(i)) Then GoTo 52
 RP12$ = " P>4Q ?"
 K = 1
52: QS(i) = QS(i) / 4
53: If (VN1(i) <= VN2(i)) Then GoTo 55
If K = 1 Then GoTo 54
RP12$ = RP12$ + " Umin > Umax ?"
GoTo 55
54: RP12$ = RP12$ + LF$ + "                                                             Umin > Umax ?"
'  Конец сервиса узлов
55:
Screen.ActiveControl = Screen.ActiveControl + RP1$ + RP2$ + RP3$ + RP4$ + RP5$ + RP6$ + RP7$ + RP8$ + RP9$ + RP10$ + RP11$ + RP12$ + LF$
   Next i
 Screen.ActiveControl = Screen.ActiveControl + LF$ + "        Информация о ветвях" + LF$
 Screen.ActiveControl = Screen.ActiveControl + "Ветвь      R      " + _
           " X     GY       BY      Ктр     Kтр прод.     P max" + LF$
      
   For i = 1 To M
  RP12$ = ""
  RP1$ = Format(NM(1, i), "######  ")
  RP2$ = Format(NM(2, i), "######  ")
  RP3$ = Format(GR(i), "##0.#0  ")
  RP4$ = Format(BX(i), "##0.#0  ")
  RP5$ = Format(GY(i), "###0.#0  ")
  RP6$ = Format(BY(i), "##0.#0  ")
  RP7$ = Format(AK(i), "###0.#0  ")
  RP8$ = Format(RK(i), "##0.#0  ")
  RP9$ = Format(PMAX(i), "##0.#0  ")
' Сюда вставить сервис ветвей
Screen.ActiveControl = Screen.ActiveControl + RP1$ + RP2$ + RP3$ + RP4$ + RP5$ + RP6$ + RP7$ + RP8$ + RP9$ + RP12$ + LF$
   Next i
'END2:
'DATE_CORR
End Sub

Private Sub GRAF1()
Dim ALARM1$, ALARM4$, NMS1$, R1$, NMS2$, ALARM2$, ALARM3$
ALARM1$ = ""
ALARM2$ = ""

'Проверяем наличие узла N для ветви N-M

For i = 1 To M
 If NM(1, i) = NSB Then GoTo FL1:
  For j = 1 To N
 If NM1(1, i) = NN(j) Then GoTo FL1:
Next j
NMS1$ = NM(1, i)
NMS2$ = NM(2, i)
ALARM1$ = ALARM1$ + "Нет узла  " + NMS1$ + " для ветви " + NMS1$ + " - " + NMS2$ + Chr$(10)
FL1: Next i

For i = 1 To M
 If NM(2, i) = NSB Then GoTo FL2:
  For j = 1 To NN
 If NM(2, i) = NN(j) Then GoTo FL2:
Next j
NMS1$ = NM(1, i)
NMS2$ = NM(2, i)
ALARM1$ = ALARM1$ + "Нет узла   " + NMS2$ + " для ветви " + NMS1$ + " - " + NMS2$ + Chr$(10)
FL2: Next i

'If ALARM1$ <> "" Then MsgBox ALARM1$, , "РАСЧЕТ НЕВОЗМОЖЕН!"

For i = 1 To N
   For j = 1 To M
   If NN(i) = NM(1, j) Then GoTo FL3:
   If NN(i) = NM(2, j) Then GoTo FL3:
Next j
NMS1$ = NAM(i)
ALARM2$ = ALARM2$ + "Нет ветви   " + " для узла " + NMS1$ + Chr$(10)
FL3: Next i
ALARM2$ = ALARM1$ + ALARM2$

If ALARM2$ <> "" Then MsgBox ALARM2$, , "РАСЧЕТ НЕВОЗМОЖЕН!"

ALARM3$ = ALARM3$ + "Нет балансирующего узла " + Chr$(10)

If NSB = 0 Then MsgBox ALARM3$, , "РАСЧЕТ НЕВОЗМОЖЕН!"

For i = 1 To M
 If GR(i) < 100 Then GoTo FL4:
R1$ = GR(i)
ALARM4$ = ALARM4$ + "Сопротивление больше нормы" + Chr$(10)
FL4: Next i
If ALARM4$ <> "" Then MsgBox ALARM4$, , "РАСЧЕТ НЕВОЗМОЖЕН!"
End Sub
'**** YM *******
Private Sub YM()
'Формирование матрицы проводимостей
Dim KOL(150), K, L, R, R1, R2, AMOD, MS, I1
'Cls
'Print ""
'Print ""
    ' Перестановка балансирующего узла на последнее место
    For i = 1 To N
    If NK(i) <> 6 Then GoTo 20
    NK(i) = NK(NO)
    L = NN(i)
    NN(i) = NN(NO)
    NN(NO) = L
'Пока процедуры нет
    'ROB3 (VN1(i), VN1(NO))
'    ROB3 (VN2(i), VN2(NO))
'    ROB3 (PS(i), PS(NO))
'    ROB3 (QS(i), QS(NO))
'    ROB3 (GG(i), GG(NO))
'    ROB3 (BB(i), BB(NO))
'    ROB3 (DN(i), DN(NO))
    GoTo 30
20:
     Next i
30:
  'Перестановка ветвей и пересчет Кт < 1
    For i = 1 To M
    R1 = GR(i) * GR(i) + BX(i) * BX(i)
    GR(i) = GR(i) / R1
    BX(i) = -BX(i) / R1
    R1 = AK(i) * AK(i) + RK(i) * RK(i)
    AMOD = Sqr(R1)
    If AMOD < 1 Then GoTo 40
    AK(i) = AK(i) / R1
    RK(i) = -RK(i) / R1
    L = NM(2, i)
    NM(2, i) = NM(1, i)
    NM(1, i) = L
40:
     Next i
  'Определение числа ветвей, примыкающих к узлу для упорядочивания
    MS = 0
    For i = 1 To N
    KOL(i) = 0
    For j = 1 To M
    If (NN(i) = NM(1, j) Or NN(i) = NM(2, j)) Then KOL(i) = KOL(i) + 1
    Next j
    If KOL(i) > MS Then MS = KOL(i)
    Next i
    I1 = 0
    For i = 1 To MS
    For j = 1 To N
    If KOL(j) <> i Then GoTo 85
    I1 = I1 + 1
    NL(I1) = NN(j)
85:
   Next j
   Next i
  'Пока здесь остановились
  'Определение машинных номеров ветвей
   For i = 1 To M
   If NM(1, i) = NN(NO) Then NM(1, i) = 0
   If NM(2, i) = NN(NO) Then NM(2, i) = 0
   For j = 1 To N
   If NM(1, i) <> NL(j) Then GoTo 90
   NM(1, i) = j
   GoTo 100
90:
   Next j
100:
 For j = 1 To N
 If NM(2, i) <> NL(j) Then GoTo 110
   NM(2, i) = j
   GoTo 120
110:
    Next j
120:
    Next i
   For i = 1 To N
   For j = 1 To N
   If NN(j) <> NL(i) Then GoTo 130
   YY(2 * i - 1) = BB(j)
   YY(2 * i) = GG(j)
   'Расчет активной, реактивной и полной составляющих напряжения

   R = (DN(j) + DN(NO)) / 57.3
   R2 = (VN1(j) + VN2(j)) / 2
   VR(i) = R2 * Sin(R)
   VA(i) = R2 * Cos(R)
  'If keyn(2) = 1 Then Print "U,Va,Vr: "; U; VA(i); VR(i)
  
130:
    Next j
    Next i
   'MsgBox "Продолжить?"
    For i = 1 To M
    K = 0
    R1 = AK(i) * AK(i) + RK(i) * RK(i)
    j = NM(1, i)
150:
    If j = 0 Then GoTo 160
    YY(2 * j - 1) = YY(2 * j - 1) + BX(i) / R1 + BY(i) / 2
    YY(2 * j) = YY(2 * j) + GR(i) / R1 + GY(i) / 2
160:
    If K <> 0 Then GoTo 170
    j = NM(2, i)
    R1 = 1
    K = 1
    GoTo 150
170:
    Next i
    
   R = DN(NO) / 57.3
   R1 = (VN1(NO) + VN2(NO)) / 2
   VSBR = R1 * Sin(R)
   VSBA = R1 * Cos(R)
''*****************
' Cтарый вариант
'Расчет взаимной активной и реактивной проводимостей

 '   If keyn(3) > 0 Then
  '             Print "Взаимные проводимости"
   '            For i = 1 To MM
    '             Print Format(Gm(i), "#0.##"); "  "; Format(Bm(i), "#0.##"):
     '          Next i
    ' End If
 
 'MsgBox "Продолжить?"

'    If keyn(3) > 0 Then
 '             Print "Собственные проводимости"
  '            For i = 1 To NN
   '              Print Format(G(i), "#0.##"); "  "; Format(B(i), "#0.##"):
    '
     '          Next i

End Sub
'' *********************************************************************


'Private Sub Newton()
    ' Основная программа расчета установившегося режима.
'DATE_CORR
''YM
'DELT2
'LOAD2
''End Sub
Private Sub LOADFLOW()
' Расчет токов и  перетоков мощности
Open FILE_OUT For Append As #1
ddp = 0    'здесь будет подсчет потерь
ddq = 0    'активной и реактивной мощностей в узлах
Print "Потоки мощности по ветвям:"
Print #1, "N1"; Spc(4); "N2"; Spc(4); "Pнач"; Spc(5); "Qнач"; Spc(5); "Pкон"; Spc(5); "Qкон"; Spc(5); "dРпот"; Spc(5); "dQпот"
Print "N1"; Spc(9); "N2"; Spc(9); "Pнач"; Spc(11); "Qнач"; Spc(11); "Pкон"; Spc(11); "Qкон"; Spc(8); "dРпот"; Spc(6); "dQпот"
    
For i = 1 To MM
  K = NM1(i)
  j = NM2(i)
  s2 = -Gm(i) / AK(i)
  am2 = -Bm(i) / AK(i)
  ajj = VA(j) * s2 + VR(j) * am2
  ai3 = VA(j) * am2 - VR(j) * s2
  u0 = VA(K) * VA(K) + VR(K) * VR(K)

  Pkj = u0 * s2 - (VA(K) * ajj - VR(K) * ai3) * AK(i)
  Qkj = u0 * am2 - (VA(K) * ai3 + VR(K) * ajj) * AK(i)

  u0 = VA(j) * VA(j) + VR(j) * VR(j)
  ajj = VA(K) * s2 + VR(K) * am2
  ai3 = VA(K) * am2 - VR(K) * s2
  Pjk = u0 * s2 * AK(i) * AK(i) - (VA(j) * ajj - VR(j) * ai3) * AK(i)
  Qjk = u0 * am2 * AK(i) * AK(i) - (VA(j) * ai3 + VR(j) * ajj) * AK(i)
' Потери в линии
  dP = Pkj + Pjk
  dQ = Qkj + Qjk
' Суммарные потери в сети
  ddp = ddp + dP
  ddq = ddq + dQ

 

  
  Nnode = Format(NAM(K), "###### ")
  Ntype = Format(NAM(j), "## ")
  Ua = Format(Pkj, "###0.#0")
  Ur = Format(Qkj, "##0.#0")
  Pnode = Format(Pjk, "###0.#0")
  Qnode = Format(Qjk, "###0.#0")
  Gnode = Format(dP, "###0.#0")
  Bnode = Format(dQ, "###0.#0")
 'Print #1, Nnode, Ntype, Ua, Ur, Pnode, Qnode, Gnode, Bnode
 
  Print #1, Nnode; Spc(4); Ntype; Spc(4); Ua; Spc(4); Ur; Spc(4); Pnode; Spc(4); Qnode; Spc(4); Gnode; Spc(4); Bnode
  Print Nnode, Ntype, Ua, Ur, Pnode, Qnode, Gnode, Bnode
 
Next i
Print " Суммарные потери активной мощности (МВт):  "; Format(ddp, "##.##");
Print #1, " Суммарные потери активной мощности (МВт):  "; Format(ddp, "##.##");
Print #1,
Print
Print " Суммарные потери реактивной мощности (МВар):  "; Format(ddq, "##.##"); Tab;
Print
Print #1, " Суммарные потери реактивной мощности (МВар):  "; Format(ddq, "##.##"); Tab;
Print #1,

'Мощность базисного узла
For i = 1 To NN
  P(NN + 1) = P(NN + 1) + P(i)
  Q(NN + 1) = Q(NN + 1) + Q(i)
Next i
P(NN + 1) = P(NN + 1) + ddp
Q(NN + 1) = Q(NN + 1) + ddq
Print "Мощность базисного узла: P="; P(NN + 1); "Q="; Q(NN + 1)
Print #1, "Мощность базисного узла: P="; P(NN + 1); "Q="; Q(NN + 1)
Close #1
End Sub

Private Sub JACOB()
For i = 1 To NN
  For j = 1 To NN
    H(i, j) = 0
  Next j
Next i
'  блоки формирования  диагональных элементов матрици  Якоби - такие
' же, как и в процедуре FUNCTION

For K = 1 To NN
' нечетная строка - для Q,
' четная - для P
  j2 = 2 * K

  sqVa = 0
  sqVr = 0
  spVa = 0
  spVr = 0
  For i = 1 To MM
    If NM1(i) = K Then j = NM2(i): GoTo M20
    If NM2(i) = K Then j = NM1(i): GoTo M20
    GoTo M21
M20:
' Все далее следующие уравнения соответствуют уравнениям (10)
' Cумма составляющих производных от прилежащих узлов
    spVa = spVa + VA(j) * Gm(i) + VR(j) * Bm(i)
    spVr = spVr + VR(j) * Gm(i) - VA(j) * Bm(i)
    sqVa = sqVa + VA(j) * Bm(i) - VR(j) * Gm(i)
    sqVr = sqVr + VR(j) * Bm(i) + VA(j) * Gm(i)

'недиагональные элементы
    j3 = 2 * j

    H(j2 - 1, j3 - 1) = VA(K) * Bm(i) + VR(K) * Gm(i)
    H(j2 - 1, j3) = VR(K) * Bm(i) - VA(K) * Gm(i)
    H(j2, j3 - 1) = VA(K) * Gm(i) - VR(K) * Bm(i)
    H(j2, j3) = VR(K) * Gm(i) + VA(K) * Bm(i)
    If Ntyp(K) = 2 Then H(j2 - 1, j3 - 1) = 0: H(j2 - 1, j3) = 0
M21:   Next i
' Окончательно диагональные элементы
  H(j2 - 1, j2 - 1) = 2 * B(K) * VA(K) + sqVa
  H(j2 - 1, j2) = 2 * B(K) * VR(K) + sqVr
  H(j2, j2 - 1) = 2 * G(K) * VA(K) + spVa
  H(j2, j2) = 2 * G(K) * VR(K) + spVr
  If Ntyp(K) = 2 Then H(j2 - 1, j2 - 1) = -2 * VA(K): H(j2 - 1, j2) = -2 * VR(K)

Next K
'Print "Матрица Якоби"
'For i = 1 To 2 * NN
'  For j = 1 To 2 * NN
'    Print Format(H(i, j), "##.##"); Tab;
'  Next j
'  Print ""
'Next i
' MsgBox "Продолжить?"

End Sub





Private Sub FUNCT()
'Вычисление вектора небалансов
'и Евклидовой нормы
' Предварительное обнуление сумм
If keyn(4) <> 0 Then Cls: Print: Print
s1 = 0
s2 = 0
ss = 0
' Для  каждого  узла  определяется  сумма  (второй  член)  уравнения
' балансов
For K = 1 To NN
  s0 = 0
  S = 0
  For i = 1 To MM
    If NM1(i) = K Then j = NM2(i): GoTo M10
    If NM2(i) = K Then j = NM1(i): GoTo M10
    GoTo M11
M10:
' Квадрат напряжения узла
    U2 = VA(K) * VA(K) + VR(K) * VR(K)
    S = S + (VA(j) * VA(K) + VR(K) * VR(j)) * Gm(i) + (VA(K) * VR(j) - VA(j) * VR(K)) * Bm(i)
    s0 = s0 + (VA(j) * VA(K) + VR(K) * VR(j)) * Bm(i) - (VA(K) * VR(j) - VA(j) * VR(K)) * Gm(i)
M11: Next i

'Вектор небалансов
'В векторе небалансов на нечетном  месте хранится поправка для Q,
'на четном - для P
  j2 = 2 * K
  w(j2) = P(K) + U2 * G(K) + S
  If Ntyp(K) = 1 Then w(j2 - 1) = Q(K) + U2 * B(K) + s0
  If Ntyp(K) = 2 Then
    w(j2 - 1) = Umax(K) * Umax(K) - VA(K) * VA(K) - VR(K) * VR(K)
    Q(K) = -U2 * B(K) - s0
  End If
  ss = ss + w(j2) * w(j2) + w(j2 - 1) * w(j2 - 1)
Next K
ss = Sqr(ss / (NN * 2))
iter = iter + 1
If keyn(4) <> 0 Then
Print "Вектор небалансов:"
For i = 1 To NN
  j = 2 * i
  Print Format(w(j - 1), "##.##"); Tab; Format(w(j), "##.##"):
Next i
MsgBox "Продолжить?"
End If

Print "Норма:"; Format(ss, "##.##"); Tab; "Итерация:"; iter ' Печатать всегда!

End Sub




Private Sub OUT_REZ()
' Cls

Print ""
Print ""
'Вывод результатов расчета
Print " Результаты расчетов схемы"
Print
Print "     Данные по узлам"
Print "N узла"; Spc(5); "Тип"; Spc(10); "Uном"; Spc(10); "Фаза"; Spc(10); _
          "P"; Spc(10); "Q"; Spc(12); "G"; Spc(13); "B"
     For i = 1 To NN
      U = Sqr(VA(i) * VA(i) + VR(i) * VR(i))
     
     Nnode = Format(NAM(i), "###### ")
  Ntype = Format(Ntyp(i), "## ")
  U = Format(U, "###0.#0")
  Ur = Format(VR(i), "##0.#0")
  Pnode = Format(P(i), "###0.#0")
  Qnode = Format(Q(i), "###0.#0")
  Gnode = Format(Gshunt(i), "###0.#0")
  Bnode = Format(Bshunt(i), "###0.#0")
  Print Nnode; Tab; Ntype; Tab; U; Tab; Ur; Tab; Pnode; Tab; Qnode; Tab _
            ; Gnode; Tab; Bnode
     Next i
Print
'Print "     Данные по ветвям"
 '    For i = 1 To MM
     
'Nnode1 = Format(NM1(i), "######")
 ' Nnode2 = Format(NM2(i), "######")
  'Rbranch = Format(R(i), "##0.#0")
  'Xbranch = Format(X(i), "##0.#0")
  'Ktr = Format(aK(i), "##0.#0")
 ' Bbranch = Format(bb(i), "###0.#0")
 ' Gbranch = Format(gg(i), "###0.#0")
 ' Print Nnode1; Tab; Nnode2; Tab; Rbranch; Tab; Xbranch _
  '          ; Tab; Ktr; Tab; Bbranch; Tab; Gbranch
'Next i

FILEoutNAME


End Sub
Private Sub FILEoutNAME()

Dim U

FILE_OUT = ""
For i = 1 To 8
F1$ = Mid$(FILE_ID, i, 1)
FILE_OUT = FILE_OUT + F1$
If F1$ = "." Then GoTo lab2
Next i
lab2: FILE_OUT = FILE_OUT + "csv"

Open FILE_OUT For Output As #1
Print #1, " Результаты расчетов схемы"
Print
Print #1, "     Данные по узлам"
 For i = 1 To NN
  U = Sqr(VA(i) * VA(i) + VR(i) * VR(i))
 Pnode = Format(P(i), "###0.#0")
  Qnode = Format(Q(i), "###0.#0")
  Gnode = Format(Gshunt(i), "###0.#0")
  Bnode = Format(Bshunt(i), "###0.#0")
  
  
  Print #1, NAM(i); Spc(4); Ntyp(i); Spc(4); Format(U, "###0.#0"); Spc(4); Format(VR(i), "##0.#0"); Spc(4); Pnode; Spc(4); Qnode; Spc(4); Gnode; Spc(4); Bnode
 Next i
 
'Print #1, "     Данные по ветвям"
' For i = 1 To MM
'    Print #1, NM1(i); NM2(i); R(i); X(i); aK(i); bb(i); gg(i)
'Next i
Close #1


End Sub
Private Sub CDU()

'Ввод информации по ветвям

  if (in(2),EQ.1) goto 10
  if (in(2),EQ.2) goto 20
  if (in(2),EQ.3) goto 30
  if (in(2),EQ.4) goto 40
  if (in(2),EQ.7) goto 50
  if (in(2),EQ.8) goto 53
  write (3,4)
4:format(40x,"не указан номер п/к ветви")
  PAUSE "не указан номер п/к ветви"
  GOTO5
10: M = M + 1
  NM(1, M) = AA(1)
  NM(2, M) = AA(2)
  GR(M) = AA(3)
  BX(M) = AA(4)
  If (BX(M) = 0) Then BX(M) = 0.001
  if(GR(M).EQ.0)GR(M)=0.001
  BY(M) = -AA(5) / 1000000#
  AK(M) = AA(6)
  RK(M) = AA(7)
  GY(M) = AA(8) * 0.00001
20: doto5
30:in(4)=in(4)+1
  M1=IN(4)
  YY(M1) = AA(1)
  ss(M1) = AA(2)
  DS1(M1) = AA(4)
40: GOTO5
50: continue
  L1 = AA(1)
  L2 = AA(2)
  DO51I=1,M
  if(NM(1,I).EQ,L1.AND.NM(2,I).EQ,L2)GOTO52
  if(NM(2,I).EQ,L1.AND.NM(1,I).EQ,L2)GOTO52
  goto51
52: UKU(1, i) = AA(3)
  UKU(2, i) = AA(4)
  QP(1, i) = AA(5)
  QP(1, i) = AA(6)
  QP(1, i) = AA(7)
  QP(1, i) = AA(8)
  GOTO5
53: continue
  SR(1, i) = AA(3)
  SX(1, i) = AA(4)
  QQ(1, i) = AA(5)
  QP(1, i) = AA(6)
  PQ(1, i) = AA(7)
  PP(1, i) = AA(8)
  SP(1, i) = AA(2)
  SG(1, i) = AA(9)
  GOTO5
51: continue
print222 , L1, L2
222:format(10x,"осутствует линия",
*14,"-",14,"для погрешностей R и X ")
5: continue


IF(IN(2).EQ.1)GOTO10
IF(IN(2).EQ.2)GOTO20
IF(IN(2).EQ.6)GOTO40
IF(IN(2).EQ.3)GOTO30
IF(IN(2).EQ.4)GOTO5
IF(IN(2).EQ.7)GOTO63
IF( IN(2).EQ,8) GO TO 66 I
WRIТЕ(3,7)
7: FORMAT(40X."НЕ УКАЗАН НОМЕР П/К УЗЛА")
PAUSE "НЕ УКАЗАН НОМЕР П/К УЗЛА"
GOT0 5
ПРИСВАИВАНИЕ ПЕРВОЙ КАРТЫ
10: N = N + 1
IN(9)=IN(9)+1
IF(lN(9),GT,26.OR.iN(9).LE.0) lN(9)=1
NN(N)=AA(1)+IN(9)*1000000
D(N) = AA(2)
QS(N)=AA{4)
PS(N) = AA(3)
QQ(N) = AA(6)
PP(N) = AA(5)
DS3(N) = AA(7)
QMIN(N) = AA(8)
QMAX(N)=AA<9)
IF(AA(8),EO.0.0.AND.AA(9).EQ.0.0) GOT0 65
IF(AA(7).NE.0)D(N)=AA(7)
NN(N) = -NN(N)
655: IF(AA(7).NE.0.)GOT065
DS3(N) = AA(2)
65: C0NTINUE
GOT05
ПРИСВАИВАНИЕ ВТОРОЙ КАРТЫ
20:IN(7)=IN(7)+l
N1=IN(7)
NK(N1) = AA(1)
IF(AA(6),NE,0)NK(N1)=-NK(N1)
VA(NL) = AA(2)
VR(NL) = AA(3)
DN(NL) = AA(9) / 57.3
30: GOT0 5
присваивание КАРТЫ погрешностей
40: IN(6)=IN(6)+1
N2=IN(6)
IF(AA(5).NE.0..OR.AA(6).NE.0..0R.AA(7)
GoTo 61
RR(N2)=l000000.+AA(2)
KN = 300 + N2
RR(KN) = 1000000# + АА(3)
UKU(4,N2)е1000000.+АА(4)
G0T062
63: continue
SP1 = AA(3)
SQl = AA(4)
PQ = AA(8)
GOT05
61: RR(N2) = AA(5)
KN = 300 + N2
RR(KN) = AA(6)
UKU(4, N2) = AA(7)
62: NL(N2) = AA(L)
UKU(3, N2) = AA(8)
GOTO5
66: DO 67 I=1,4
IF(AA(2*I-1).FQ.0) CO T067
NZ(5l)=NZ(5l)+1
NZZ = NZ(51)
NZ(NZZ) = АА(i * 2 - 1)
PZ(NZZ) = AA(2 * i)
67: continue
5: continue
Return
End
End Sub

End Sub

