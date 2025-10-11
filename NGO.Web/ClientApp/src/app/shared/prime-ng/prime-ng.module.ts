import { NgModule } from '@angular/core';
import { ConfirmationService } from 'primeng/api';
import { MessageService } from 'primeng/api';
import { ButtonModule } from 'primeng/button';
import { CalendarModule } from 'primeng/calendar';
import { CardModule } from 'primeng/card';
import { ChartModule } from 'primeng/chart';
import { CheckboxModule } from 'primeng/checkbox';
import { ConfirmDialogModule } from 'primeng/confirmdialog';
import { ContextMenuModule } from 'primeng/contextmenu';
import { DialogModule } from 'primeng/dialog';
import { DropdownModule } from 'primeng/dropdown';
import { InputSwitchModule } from 'primeng/inputswitch';
import { InputTextModule } from 'primeng/inputtext';
import { InputTextareaModule } from 'primeng/inputtextarea';
import { MultiSelectModule } from 'primeng/multiselect';
import { PanelModule } from 'primeng/panel';
import { ProgressBarModule } from 'primeng/progressbar';
import { ProgressSpinnerModule } from 'primeng/progressspinner';
import { RadioButtonModule } from 'primeng/radiobutton';
import { SliderModule } from 'primeng/slider';
import { TableModule } from 'primeng/table';
import { ToastModule } from 'primeng/toast';

@NgModule({
  declarations: [],
  imports: [
    TableModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    CalendarModule,
    CheckboxModule,
    RadioButtonModule,
    DropdownModule,
    ConfirmDialogModule,
    InputSwitchModule,
    MultiSelectModule,
    ProgressSpinnerModule,
    ToastModule,
    SliderModule,
    ContextMenuModule,
    ProgressBarModule,
    ChartModule,
    PanelModule,
    CardModule,
  ],
  exports: [
    TableModule,
    DialogModule,
    ButtonModule,
    InputTextModule,
    InputTextareaModule,
    CalendarModule,
    MultiSelectModule,
    CheckboxModule,
    RadioButtonModule,
    DropdownModule,
    ConfirmDialogModule,
    InputSwitchModule,
    ProgressSpinnerModule,
    ToastModule,
    SliderModule,
    ContextMenuModule,
    ProgressBarModule,
    ChartModule,
    PanelModule,
    CardModule,
  ],
  providers: [ConfirmationService, MessageService],
})
export class PrimeNGModule {}
