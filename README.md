**DelApplication** - Простая консольная программа для автоматического удаления \*.msi, \*.exe программ, а так же конкретных файлов и папок.

Утилита предназначена для упрощения автоматизации и администрирования, как самостоятельно, так и для совместной работы с другим моим проектом DAVrun, ссылка на [GitHub](https://github.com/Otto17/DAVrun) и [GitFlic](https://gitflic.ru/project/otto/davrun).

_Данная программа является свободным программным обеспечением, распространяющимся по лицензии MIT._

---

_**Что может данная утилита:**_

Поддерживает 3 способа удаления для программ с расширениями \*.exe и \*.msi:

1.  По имени файла (поиск осуществляется из реестра), используется ключ "**/N**" или "**/NAME**".
2.  По ID (из реестра) установленной программы, используется ключ "**/R**" или "**/REGISTRY**".
3.  По полному пути к файлу программы удаления "unins000.exe", используется ключ "**/P**" или "**/PATH**".

_**Дополнительно:**_

*   Поддерживаются опциональные ключи от удаляемых программ, если таковы есть (например для тихого удаления без перезагрузки), указываются в самом конце после вызова "**/K**" или "**/KEY**" .
*   Так же поддерживается удаление любых папок или файлов, то есть можно удалять "**Portable**" программы, для этого используется ключ "**/D**" или "**/DELETE**".

```plaintext
Разные программы могут не поддерживать тот или иной способ удаления, поэтому способов больше, чем один.
```

---

**Использование: DelApplication.exe \<Ключ> \<Значение> \[/KEY \<Ключи для удаления (не обязательно)>\]**

---

**Примеры использования на реальных программах.**

_Программа "WinSCP" для подключения через SCP, FTP, SFTP, S3 и WebDAV:_

*   [x] (\*.exe) - DelApplication /N "WinSCP" /KEY “/VERYSILENT”
*   [x] (\*.exe) - DelApplication /PATH "C:\\Users\\user\\AppData\\Local\\Programs\\WinSCP\\unins000.exe" /KEY “/VERYSILENT”
*   [x] (\*.msi) - DelApplication /NAME "WinSCP" /K “/quiet”
*   [x] (\*.msi) - DelApplication /R "MsiExec.exe /X{7F02DF31-4309-4D68-B740-C3ED6F48FF9C}" /K “/quiet”

_Программа "Samsung Magician" для работы с SSD:_

*   [x] DelApplication.exe /PATH "C:\\Program Files (x86)\\Samsung\\Samsung Magician\\unins000.exe" /KEY "/VERYSILENT /NORESTART"

_Программа удалённого доступа "Ассистент":_

*   [x] DelApplication.exe /NAME "Ассистент" /K "/quiet"

_Удаление конкретного файла или папки со всем её содержимым:_

*   [x] DelApplication.exe /DELETE “D:\\Шлак\\Тестовая папка\\tst OK\\DelApplication.pdb”
*   [x] DelApplication /D "C:\\Users\\Public\\Desktop\\Тестовая папка"

**Запуск программы без аргументов выведет справку.**

---

**Автор Otto, г. Омск 2024**