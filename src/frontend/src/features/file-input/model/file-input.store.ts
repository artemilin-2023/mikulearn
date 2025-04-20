import { makeAutoObservable } from 'mobx';

class FileInputStore {
  file: File | null = null;
  isPreviewOpen: boolean = false;

  constructor() {
    makeAutoObservable(this);
  }

  setFile(file: File) {
    this.file = file;
  }

  togglePreview() {
    this.isPreviewOpen = !this.isPreviewOpen;
  }

  clearFile() {
    this.file = null;
    this.isPreviewOpen = false;
  }

  openPreview() {
    if (!this.file) {
      return;
    }
    this.isPreviewOpen = true;
  }

  closePreview() {
    this.isPreviewOpen = false;
  }

  get hasFile(): boolean {
    return !!this.file;
  }
}

export const fileInputStore = new FileInputStore();
