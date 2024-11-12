import { Component, OnInit, ViewChild } from '@angular/core';
import { NgbModal } from '@ng-bootstrap/ng-bootstrap';
import { MessageService, ConfirmationService } from 'primeng/api';
import { Table } from 'primeng/table';

export interface Product {
  id?: string;
  code?: string;
  name?: string;
  description?: string;
  price?: number;
  quantity?: number;
  inventoryStatus?: string;
  category?: string;
  image?: string;
  rating?: number;
}

@Component({
  selector: 'app-registersecond',
  templateUrl: './registersecond.component.html',
  styleUrls: ['./registersecond.component.css'],
  providers: [MessageService, ConfirmationService]
})
export class RegistersecondComponent implements OnInit {
  productDialog: boolean = false;
  products!: Product[];
  product!: Product;
  selectedProducts!: Product[] | null;
  submitted: boolean = false;
  statuses: any[] | undefined;
  @ViewChild('dt') dt: Table | undefined;  // Reference to the p-table component

  // Sample product data
  sampleProducts: Product[] = [
    {
      id: '1000',
      code: 'f230fh0g3',
      name: 'Bamboo Watch',
      description: 'Product Description',
      image: 'bamboo-watch.jpg',
      price: 65,
      category: 'Accessories',
      quantity: 24,
      inventoryStatus: 'INSTOCK',
      rating: 5
    },
    {
      id: '1001',
      code: 'x234g7hj2',
      name: 'Black Watch',
      description: 'Product Description',
      image: 'black-watch.jpg',
      price: 45,
      category: 'Accessories',
      quantity: 18,
      inventoryStatus: 'LOWSTOCK',
      rating: 4
    },
    {
      id: '1002',
      code: 'g245thf76',
      name: 'Red Watch',
      description: 'Product Description',
      image: 'red-watch.jpg',
      price: 85,
      category: 'Accessories',
      quantity: 30,
      inventoryStatus: 'INSTOCK',
      rating: 4
    },
    {
      id: '1003',
      code: 'm134gh4j6',
      name: 'Yellow Watch',
      description: 'Product Description',
      image: 'yellow-watch.jpg',
      price: 55,
      category: 'Accessories',
      quantity: 10,
      inventoryStatus: 'OUTOFSTOCK',
      rating: 3
    }
  ];

  constructor(
    private modalService: NgbModal,
    private messageService: MessageService,
    private confirmationService: ConfirmationService
  ) {}

  ngOnInit() {
    // Use the dummy sample data
    this.products = this.sampleProducts;

    this.statuses = [
      { label: 'INSTOCK', value: 'INSTOCK' },
      { label: 'LOWSTOCK', value: 'LOWSTOCK' },
      { label: 'OUTOFSTOCK', value: 'OUTOFSTOCK' }
    ];
  }

  openModal(content: any) {
    this.modalService.open(content, { ariaLabelledBy: 'modal-basic-title' });
  }

  filterGlobal(event: Event): void {
    const inputElement = event.target as HTMLInputElement;
    const value = inputElement?.value || '';
    if (this.dt) {
      this.dt.filterGlobal(value, 'contains');  // Use dt reference to apply the filter
    }}

  openNew() {
    this.product = {} as Product;
    this.submitted = false;
    this.productDialog = true;
  }

  deleteSelectedProducts() {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete the selected products?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.products = this.products.filter((val) => !this.selectedProducts?.includes(val));
        this.selectedProducts = null;
        this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Products Deleted', life: 3000 });
      }
    });
  }

  editProduct(product: Product) {
    this.product = { ...product };
    this.productDialog = true;
  }

  deleteProduct(product: Product) {
    this.confirmationService.confirm({
      message: 'Are you sure you want to delete ' + product.name + '?',
      header: 'Confirm',
      icon: 'pi pi-exclamation-triangle',
      accept: () => {
        this.products = this.products.filter((val) => val.id !== product.id);
        this.product = {} as Product;
        this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Product Deleted', life: 3000 });
      }
    });
  }

  hideDialog() {
    this.productDialog = false;
    this.submitted = false;
  }

  saveProduct() {
    this.submitted = true;
    if (this.product.name?.trim()) {
      if (this.product.id) {
        this.products[this.findIndexById(this.product.id)] = this.product;
        this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Product Updated', life: 3000 });
      } else {
        this.product.id = this.createId();
        this.product.image = 'product-placeholder.svg';
        this.products.push(this.product);
        this.messageService.add({ severity: 'success', summary: 'Successful', detail: 'Product Created', life: 3000 });
      }

      this.products = [...this.products];
      this.productDialog = false;
      this.product = {} as Product;
    }
  }

  findIndexById(id: string): number {
    let index = -1;
    for (let i = 0; i < this.products.length; i++) {
      if (this.products[i].id === id) {
        index = i;
        break;
      }
    }
    return index;
  }

  createId(): string {
    let id = '';
    var chars = 'ABCDEFGHIJKLMNOPQRSTUVWXYZabcdefghijklmnopqrstuvwxyz0123456789';
    for (var i = 0; i < 5; i++) {
      id += chars.charAt(Math.floor(Math.random() * chars.length));
    }
    return id;
  }

  getSeverity(status: any) {
    switch (status) {
      case 'INSTOCK':
        return 'success';
      case 'LOWSTOCK':
        return 'warning';
      case 'OUTOFSTOCK':
        return 'danger';
    }
    return 'danger';

  }

 
}
