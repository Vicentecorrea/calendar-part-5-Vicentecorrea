## Calendario con interfaz gráfica - parte 5
### Proyecto del curso Diseño de Software Verificable

# Cambios realizados con respecto a la parte 4

* Los 2 errores de estándar de código sobre *Language Usage* fueron solucionados. Estos errores eran *"Avoid direct casts. Instead, use the “as” operator and check for null"*, ubicados en *UserController.cs* línea 63 y *CalendarForm.cs* línea 289.
* El tercer error, sobre *Formatting* de estándar de código, no fue solucionado. Lo anterior es porque si este error es solucionado, entonces aparece otro error de *Stepdown rule*. El método público *ShowSelectedDisplay()* está declarado debajo de métodos privados porque éstos llaman al método *ShowSelectedDisplay()*. Es decir, se está respetando la regla de *Stepdown*. Cabe destacar que el método *ShowSelectedDisplay()* es público porque se usa también en otras clases. Hablé con el profesor sobre este tema y me dijo que se debía priorizar el cumplimiento de la *Stepdown rule*, y que no debería haber descuento por este error. Gracias por la consideración.
* Los 21 tests siguen pasando, pero con el aumento de código (para solucionar los 2 problemas mencionados anteriormente) el % de cobertura de los tests disminuyó un poco, llegando a **25.58%** (todavía sobre el % requerido). Recordar usar el archivo *TestsCoverage.runsettings* para ejecutar los tests.

Con respecto al análisis estático, las advertencias no cambiaron, por lo que tampoco lo hicieron los 2 archivos *.xslx*

## Vicente Correa