   M S B A M L   `   `   `  ����   	clientTCP    clientTCP.MainWindow   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     -     �Name$  LoginWindows�� � Title$ Sharebox Login��$	��419��$	��480�� � 
ResizeMode$ NoResize=� � 	Resources  
  Color1��%   &	        ��	#FFD7E4F2��.���  J� RenderTransformOrigin$ 0.5,0.5&�$��0q� � ColumnDefinitions �� �� $
��236* �����  �� Color$ #FFFFFF��.+��� -    ��Name$ image��$3��-pack://siteoforigin:,,,/Icon/MainIconName.png��$
��Left=�$	��104��$��	96,10,0,0q�$	��Top=�$	��366��
 � Tag$	 asd��$��1���� -   	 ��Name$	 usernameTxtBox��$
��Left=�$��30��$��
60,147,0,0q�$	��Top=�$	��345��
 �� TextWrapping$

 Wrap=�$��1�� �� 	IsEnabled . $��18(��� -   $��Connect��$��LOGIN��$
��Left=�$��142,269,0,0q�$	��Top=�$	��189��$	 1,1&�$��30��$��18(�$��Condensed Demi-bold)�
�������$
��Bold"�$��1��$��Center=�$��Center=�$��RightToLeft=�$��1,1,1,-2q����	#FF2D86CF J� OpacityMask �	#FF870707G� -    G�Name$ paswordTxtBox��$
��Left=�$��
60,212,0,0q�$	��Top=�$��30��$	��345��$��1��$��18(��� $��USERNAME��$
��Left=�$��
60,119,0,0q�$	��Top=�$��28��$��Verdana)�$��18(����	#FF4B4444$��1���� $��PASSWORD��$
��Left=�$��
60,182,0,0q�$	��Top=�$��35��$��Verdana)�$��18(����	#FF4B4444$��1���� -    ��Name$ registerLabel��$��Sing In!��$��208,304,206,59q�$��Center=�$��27��$��58��$��Center=�$��Calibri)�$��15(����	#FF2D6B9E$��1���� -   $ errorBox��$%��Username o Password non validi!��
�������$��12(����	#FFF01818$��Verdana                )�$��1q�$��44��$��Center=�$��132,336,123,10q�$��1��$ 0.5,0.5&�$��Center=�$	��217�� J� IsHitTestVisible . $��Center=�$��Center=�$y�Hidden=��� -   $ errorBox_Password��$��Password Missing!��
�������$��12(�
���  ��$��Verdana                )�$��1q�$��30��$��Center=�$��180,192,159,168q�$��1��$ 0.5,0.5&�$��Center=�$	��133�� . $��Center=�$��Hidden=�$��Hidden=�$y�Hidden=��� -	   $ errorBox_Username��$��Username Missing!��
�������$��13(�
���  ��$��Verdana                )�$��1q�$��28��$��Center=�$��177,128,141,234q�$��1��$ 0.5,0.5&�$��Center=�$	��154�� .  $��Center=�$��Hidden=�$��Hidden=�{�.  $y�Hidden=�