export interface NavPermission {
    code: string;
    roles: ButtonPermission[]
}

export interface ButtonPermission {
    name: string;
    read: string;
    write: string;
    delete: string;
}