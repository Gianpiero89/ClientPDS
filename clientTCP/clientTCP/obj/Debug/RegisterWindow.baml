   M S B A M L   `   `   `  ����  	clientTCP    clientTCP.RegisterWindow   5   	   \ XPresentationFramework, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35R NWindowsBase, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35W SPresentationCore, Version=4.0.0.0, Culture=neutral, PublicKeyToken=31bf3856ad364e35D 9http://schemas.microsoft.com/winfx/2006/xaml/presentation    5   	   R NSystem.Xaml, Version=4.0.0.0, Culture=neutral, PublicKeyToken=b77a5c561934e089:x,http://schemas.microsoft.com/winfx/2006/xaml     5   	   -   5          �Name$  RegisterWindoww��5   	    � Title$ Register��5   	   $	��419��6   $	��480��6'   .��5      �  J� RenderTransformOrigin$ 0.5,0.5&�6   $��0q�6+   .+�5   
   �� -   5         ��Name$ image��5      $3��-pack://siteoforigin:,,,/Icon/MainIconName.png��6    $
��Left=�6W   $	��104��6r   $��	96,10,0,0q�6   $	��Top=�6�   $	��366��6�   
 � Tag$	 asd��6�   $��1��6�   �� 5	   
   -   5         ��Name$ RegisterButton��5	      $��REGISTER��6)   $
��Left=�6<   $��144,258,0,0q�6W   $	��Top=�6l   $	��189��6�   $��Condensed Demi-bold)�6�   $��18(�6�   ���	#FF2D86CF$��30��6�   
�������$
��Bold"�6  G� 5
   
   -   5         G�Name$ passwordTxtBox��5
      $
��Left=�6.   $��62,209,-43,-22q�6I   $	��Top=�6a   $��30��6y   $	��345��6�   $��18(�6�   �� 5   
   $��USERNAME��6   $
��Left=�6#   $��
62,119,0,0q�6>   $	��Top=�6R   $��28��6j   $��Verdana)�6v   $��18(�6�   ���	#FF4B44446�   �� 5   
   $��PASSWORD��6   $
��Left=�6#   $��
62,182,0,0q�6>   $	��Top=�6R   $��35��6j   $��Verdana)�6v   $��18(�6�   ���	#FF4B44446�   �� 5   
   -   5         ��Name$ usernameTxtBox��5      $
��Left=�6*   $��30��6E   $��62,147,-43,0q�6Q   $	��Top=�6g   $	��345��6    �� TextWrapping$
 Wrap=�6�   	 �� 	IsEnabled	 . $��18(�6�   �� 5   
   -   5        $ Cancel��5      $��CANCEL��6!   $
��Left=�62   $��144,306,0,0q�6M   $	��189��6b   $��Condensed Demi-bold)�6�   $��18(�6�   ���	#FF2D86CF
�������$
��Bold"�6�   $��30��6�   $	��Top=�6�   �� 5   
   -   5        
 ��Name$
 errorBox��5      $%��Username o Password non validi!��6"   
�������$��12(�6_   ���	#FFF01818$��Verdana                )�6�   $��1q�6�   $��32��6�   $��Center=�6�   $��131,346,124,10q�6�   $ 0.5,0.5&�6�   $��Center=�6  $	��217��69   J� IsHitTestVisible . $��Center=�6]  $��Center=�6�  $y�Hidden=�6�  �� 5   
   -   5        $
 errorBox_Username��5      $�� Username Missing!��6+   
�������$��13(�6[   
���  ��$��Verdana                )�6z   $��1q�6�   $��28��6�   $��Center=�6�   $��174,123,144,237q�6�   $ 0.5,0.5&�6�   $��Center=�6  $	��154��60   .  $��Center=�6U  $��Disabled=�6w  {�.  $��5,5,5,-2q�6�  $y�Hidden=�6�  �� 5   
   -	   5        $
 errorBox_Password��5      $��Password Missing!��6+   
�������$��12(�6Z   
���  ��$��Verdana                )�6y   $��1q�6�   $��30��6�   $��Center=�6�   $��179,183,160,175q�6�   $ 0.5,0.5&�6�   $��Center=�6  $	��133��6/   . $��Center=�6S  $��Hidden=�6u  $��Hidden=�6�  $��5,5,5,0q�6�  $y�Hidden=�6�  5      5      