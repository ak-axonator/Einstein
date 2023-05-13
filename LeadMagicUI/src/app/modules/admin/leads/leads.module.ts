import { NgModule } from '@angular/core';
import { Route, RouterModule } from '@angular/router';
import { LeadsComponent } from 'app/modules/admin/leads/leads.component';

const leadRoutes: Route[] = [
    {
        path     : '',
        component: LeadsComponent
    }
];

@NgModule({
    declarations: [
        LeadsComponent
    ],
    imports     : [
        RouterModule.forChild(leadRoutes)
    ]
})
export class LeadsModule
{
}
