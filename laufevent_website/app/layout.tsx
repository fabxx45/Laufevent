import { MantineProvider } from '@mantine/core';
import type { Metadata } from 'next';
import { Inter } from 'next/font/google';
import './css/globals.css';

const inter = Inter({ subsets: ['latin'] });

export const metadata: Metadata = {
    title: 'SZ-Läuft',
    description: 'Ein Laufevent für den Guten Zweck',
};

export default function RootLayout({
                                       children,
                                   }: {
    children: React.ReactNode;
}) {
    return (
        <html lang="de">
        <body className={inter.className}>
        <MantineProvider>
            {children}
        </MantineProvider>
        </body>
        </html>
    );
}