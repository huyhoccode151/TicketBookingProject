export type ToastType = 'success' | 'error' | 'warning' | 'info' | 'loading';
export type ToastPosition = 'top-right' | 'top-left' | 'top-center' | 'bottom-right' | 'bottom-left' | 'bottom-center';

export interface Toast {
  id: string;
  type: ToastType;
  title: string;
  message?: string;
  duration?: number;       // ms, 0 = persistent
  position?: ToastPosition;
  closable?: boolean;
  action?: {
    label: string;
    callback: () => void;
  };
  progress?: boolean;      // show countdown progress bar
}

export interface ToastConfig {
  defaultDuration: number;
  defaultPosition: ToastPosition;
  maxToasts: number;
}
