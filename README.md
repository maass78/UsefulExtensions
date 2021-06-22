# UsefulExtensions
Библиотека, позволяющая очень просто выполнять действия, часто необходимые в создании чекеров/регеров и прочего софта для автоматизации каких-либо процессов на сайтах.
# Features
- Решение капчи через сервисы [RuCaptcha](https://rucaptcha.com) и [AntiCaptcha](https://anti-captcha.com)
- Интерфейс взаимодействия с сервисами смс-активации ([smshub](https://smshub.org/), [sms-activate](https://sms-activate.ru/), [5sim](https://5sim.net/), [vak-sms](https://vak-sms.com/))
- Парс аккаунтов из строки, из файла (формат `login:password` или `login;password`)
- Парс проксей из строки, из файла, по ссылке на список прокси (формат `ip:port:login:password`)
# Examples
### Капча
Предположим, мы решаем капчу на сайте https://www.google.com/recaptcha/api2/demo. Вид капчи на сайте - google reCaptcha V2.
Для начала определяем решатель капчи. Сделать это можно двумя способами:

```
ICaptchaSolver rucaptchaSolver = new RucapthcaSolver("ваш апи ключ на сервисе"); 
ICaptchaSolver anticapthcaSolver = new AntiCaptchaSolver("ваш апи ключ на сервисе");
```
или
```
CaptchaSolverType rucaptchaType = CaptchaSolverType.Rucaptcha;
ICaptchaSolver rucaptchaSolver = rucaptchaType.GetCaptchaSolverByType("ваш апи ключ на сервисе");

CaptchaSolverType anticapthcaSolver = CaptchaSolverType.AntiCaptcha;
ICaptchaSolver anticapthcaSolver = anticapthcaSolver.GetCaptchaSolverByType("ваш апи ключ на сервисе");
```
Второй способ удобно использовать при работе с визуальным интерфейсом (в WPF/WinForms элемент ComboBox)

Теперь непосредственно решим капчу:
```
ICaptchaSolver rucaptchaSolver = new RucapthcaSolver("ваш апи ключ на сервисе"); 
string gRecaptchaResponse = rucaptchaSolver.SolveRecaptchaV2("6Le-wvkSAAAAAPBMRTvw0Q4Muexq9bi0DJwx_mJ-", "https://www.google.com/recaptcha/api2/demo", false);
```
Подробное описание по параметрам методов можно посмотреть в подсказках к ним. Также это хорошо описано в [документации к api рукапчи](https://rucaptcha.com/api-rucaptcha)
